using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dragon.Mail.Impl;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using Dragon.Mail.Utils;

namespace Dragon.Mail
{
    public class MailGeneratorService : IMailGeneratorService
    {
        public const string APP_KEY_ASYNCACTIVE = "Dragon.Mail.AsyncActive";

        private readonly Func<Template, IRenderer> m_rendererFactory;
        private readonly IMailQueue m_queue;
        private readonly IReceiverMapper m_receiverMapper;
        private readonly ISenderConfiguration m_senderConfiguration;
        private readonly IDataDecorator[] m_decorators;
        private readonly IMailSenderService m_mailSenderService;
        private readonly bool m_asyncActive = false;

        private Dictionary<string, Template> m_templates;
        private Dictionary<string, IRenderer> m_renderer;

        public MailGeneratorService(
            IMailQueue queue = null,
            IDataStore dataStore = null,
            Func<Template, IRenderer> rendererFactory = null,
            IDataDecorator[] decorators = null,
            IReceiverMapper receiverMapper = null,
            ISenderConfiguration senderConfiguration = null,
            IConfiguration configuration = null,
            IHttpClient httpClient = null,
            IMailSenderService mailSenderService = null,
            bool? async = null)
        {
            m_queue = queue ?? new InMemoryMailQueue();

            IConfiguration configuration1 = configuration ?? new DefaultConfiguration();

            m_rendererFactory = rendererFactory ?? (t => new HandlebarsRenderer(t));
            m_decorators = decorators ?? new IDataDecorator[]
            {
                new RestResolvingDecorator(
                    httpClient ?? new JsonHttpClient(),
                    dataStore)
            };
            m_receiverMapper = receiverMapper ?? new DefaultReceiverMapper();
            m_senderConfiguration = senderConfiguration ??
                new DefaultSenderConfiguration(
                    configuration1);

            m_asyncActive = async.HasValue ? async.Value : StringToBoolUtil.Interpret(configuration1.GetValue(APP_KEY_ASYNCACTIVE));

            if (!m_asyncActive && mailSenderService == null)
            {
                m_mailSenderService = new MailSenderService(m_queue);
            }
            else
            {
                m_mailSenderService = mailSenderService;
            }
        }

        public void Register(Template template)
        {
            m_templates = m_templates ?? new Dictionary<string, Template>();
            m_renderer = m_renderer ?? new Dictionary<string, IRenderer>();


            var localizedKey = template.LocalizedKey.ToLower();
            if (m_templates.ContainsKey(localizedKey))
            {
                System.Diagnostics.Trace.TraceWarning($"Template with identical key '{localizedKey}' already added, will override existing template.");
            }

            RegisterWithKey(localizedKey, template);

            var shorterLocalizedKey = localizedKey.Substring(0, localizedKey.LastIndexOf('-')).ToLower();
            if (m_templates.ContainsKey(shorterLocalizedKey))
            {
                System.Diagnostics.Trace.TraceWarning($"Template with identical key '{shorterLocalizedKey}' already added, will override existing template.");
            }

            RegisterWithKey(shorterLocalizedKey, template);

            var templateKey = template.Key.ToLower();
            if (m_templates.ContainsKey(templateKey))
            {
                System.Diagnostics.Trace.TraceWarning($"Template with identical key '{localizedKey}' already added, will override existing template.");
            }

            RegisterWithKey(templateKey, template);
        }

        private void RegisterWithKey(string key, Template template)
        {
            if (m_templates.ContainsKey(key))
            {
                m_templates.Remove(key);
            }
            m_templates.Add(key, template);

            if (m_renderer.ContainsKey(key))
            {
                m_renderer.Remove(key);
            }
            m_renderer.Add(key, m_rendererFactory(template));
        }

        public void Send(dynamic receiver, string templateKey, dynamic data, CultureInfo language = null)
        {
            language = (language ?? CultureInfo.CurrentCulture);
            templateKey = (templateKey ?? string.Empty).Trim().ToLower();

            if (string.IsNullOrWhiteSpace(templateKey))
            {
                throw new ArgumentException("You must specify the template key.", "templateKey");
            }

            var decoratedData = data;
            foreach (var decorator in m_decorators)
            {
                decoratedData = decorator.Decorate(decoratedData);
            }

            var decoratedReceiver = receiver;
            foreach (var decorator in m_decorators)
            {
                decoratedReceiver = decorator.Decorate(decoratedReceiver);
            }

            decoratedData.recipient = decoratedReceiver;

            IRenderer applicableRenderer = null;

            // try full name key-en-us
            var triedKeys = new List<string>();
            var localizedKey = Template.GenerateLocalizedKey(templateKey, language);
            triedKeys.Add(localizedKey);
            if (m_renderer.ContainsKey(localizedKey))
            {
                applicableRenderer = m_renderer[localizedKey];
            }

            // try shorter name key-en
            var shorterLocalizedKey = localizedKey.Substring(0, localizedKey.LastIndexOf('-'));
            triedKeys.Add(shorterLocalizedKey);
            if (applicableRenderer == null && m_renderer.ContainsKey(shorterLocalizedKey))
            {
                applicableRenderer = m_renderer[templateKey];
            }

            // try just key
            triedKeys.Add(templateKey);
            if (applicableRenderer == null && m_renderer.ContainsKey(templateKey))
            {
                applicableRenderer = m_renderer[templateKey];
            }

            if (applicableRenderer == null)
            {
                throw new ArgumentException(
                    string.Format("No template could be found for keys: {0}",
                    string.Join(",", triedKeys))
                    , "templateKey");
            }

            var mail = new Models.Mail();
            m_senderConfiguration.Configure(mail);
            m_receiverMapper.Map(decoratedReceiver, mail);

            applicableRenderer.Render(mail, decoratedData);

            m_queue.Enqueue(mail, decoratedReceiver);

            if (!m_asyncActive)
            {
                m_mailSenderService.ProcessNext();
            }
        }
    }
}
