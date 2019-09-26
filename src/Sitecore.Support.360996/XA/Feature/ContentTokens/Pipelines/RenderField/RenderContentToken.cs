using Sitecore.Data.Items;
using Sitecore.XA.Feature.ContentTokens;

namespace Sitecore.Support.XA.Feature.ContentTokens.Pipelines.RenderField
{
    public class RenderContentToken : Sitecore.XA.Feature.ContentTokens.Pipelines.RenderField.RenderContentToken
    {
        protected override string GetTextVariableValue(string variableKey)
        {
            if (!string.IsNullOrWhiteSpace(variableKey))
            {
                Item variable = Sitecore.Context.Database.SelectSingleItem($"fast://*[@@templateid='{Templates.ContentToken.ID}' and @#Key#='{variableKey}']");
                if (variable != null)
                {
                    return variable[Templates.ContentToken.Fields.Value];
                }
            }
            return string.Empty;
        }
    }
}