using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FlatFXCore.BussinessLayer
{
    /*public class TooltipAttribute : DescriptionAttribute
    {
        public TooltipAttribute()
            : base("")
        {

        }

        public TooltipAttribute(string description)
            : base(description)
        {

        }
    }

    public static class HtmlHelpers
    {
        public static MvcHtmlString ToolTipFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var exp = (MemberExpression)expression.Body;
            foreach (Attribute attribute in exp.Expression.Type.GetProperty(exp.Member.Name).GetCustomAttributes(true))
            {
                if (typeof(DescriptionAttribute) == attribute.GetType())
                {
                    return MvcHtmlString.Create(((DescriptionAttribute)attribute).Description);
                }
            }
            return MvcHtmlString.Create("");
        }
    }*/
}
