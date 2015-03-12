using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Errors;
using Dragon.CPR.Interfaces;
using Dragon.Data.Interfaces;
using FluentValidation;

namespace Dragon.CPR
{
    public class AbstractValidatorHandlerBase<T> : AbstractValidator<T>,
        IHandler<T>
        where T : CommandBase
    {
        public IEnumerable<ErrorBase> Handle(T obj)
        {
            var res = this.Validate(obj);

            return res.Errors.Select(x => new ValidationError()
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                });
        }

        public int Order { get { return 50; } }
    }
}
