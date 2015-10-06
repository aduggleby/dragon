using System;
using Dragon.Mail.Interfaces;
using Dragon.Mail.Models;
using HandlebarsDotNet;

namespace Dragon.Mail.Impl
{
    public class HandlebarsRenderer : IRenderer
    {
        private readonly Func<object, string> m_compiledTemplateBody;
        private readonly Func<object, string> m_compiledTemplateTextBody;

        private readonly Func<object, string> m_compiledTemplateSubject;
        private readonly Func<object, string> m_compiledTemplateSummaryBody;
        private readonly Func<object, string> m_compiledTemplateSummaryHeader;
        private readonly Func<object, string> m_compiledTemplateSummaryFooter;
        private readonly Func<object, string> m_compiledTemplateSummaryTextBody;
        private readonly Func<object, string> m_compiledTemplateSummaryTextHeader;
        private readonly Func<object, string> m_compiledTemplateSummaryTextFooter;

        private readonly Func<object, string> m_compiledTemplateSummarySubject;

        public HandlebarsRenderer(Template t)
        {
            m_compiledTemplateBody = Handlebars.Compile(t.Content.Body ?? String.Empty);
            m_compiledTemplateTextBody = Handlebars.Compile(t.Content.TextBody ?? String.Empty);

            m_compiledTemplateSubject = Handlebars.Compile(t.Content.Subject ?? String.Empty);
            m_compiledTemplateSummaryBody = Handlebars.Compile(t.Content.SummaryBody ?? String.Empty);
            m_compiledTemplateSummaryHeader = Handlebars.Compile(t.Content.SummaryHeader ?? String.Empty);
            m_compiledTemplateSummaryFooter = Handlebars.Compile(t.Content.SummaryFooter ?? String.Empty);
            m_compiledTemplateSummaryTextBody = Handlebars.Compile(t.Content.SummaryTextBody ?? String.Empty);
            m_compiledTemplateSummaryTextHeader = Handlebars.Compile(t.Content.SummaryTextHeader ?? String.Empty);
            m_compiledTemplateSummaryTextFooter = Handlebars.Compile(t.Content.SummaryTextFooter ?? String.Empty);
            m_compiledTemplateSummarySubject = Handlebars.Compile(t.Content.SummarySubject ?? String.Empty);
        }

        public string RenderSubject(dynamic data)
        {
            return m_compiledTemplateSubject(data);
        }

        public void Render(Models.Mail mail, dynamic data)
        {
            var sanitizedHtml = data;

            mail.Subject = m_compiledTemplateSubject(sanitizedHtml);
            mail.Body = m_compiledTemplateBody(sanitizedHtml);
            mail.TextBody = m_compiledTemplateTextBody(sanitizedHtml);

            mail.SummaryBody = m_compiledTemplateSummaryBody(sanitizedHtml);
            mail.SummaryHeader = m_compiledTemplateSummaryHeader(sanitizedHtml);
            mail.SummaryFooter = m_compiledTemplateSummaryFooter(sanitizedHtml);

            var strippedText = data;

            mail.SummarySubject = m_compiledTemplateSummarySubject(strippedText);

            mail.SummaryTextBody = m_compiledTemplateSummaryTextBody(strippedText);
            mail.SummaryTextHeader = m_compiledTemplateSummaryTextHeader(strippedText);
            mail.SummaryTextFooter = m_compiledTemplateSummaryTextFooter(strippedText);
        }
    }
}
