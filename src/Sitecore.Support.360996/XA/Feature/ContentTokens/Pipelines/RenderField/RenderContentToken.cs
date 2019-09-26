using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.XA.Feature.ContentTokens;

namespace Sitecore.Support.XA.Feature.ContentTokens.Pipelines.RenderField
{
    public class RenderContentToken : Sitecore.XA.Feature.ContentTokens.Pipelines.RenderField.RenderContentToken
    {
        private static readonly Dictionary<string, ConcurrentDictionary<string, Item>> ContentTokenCaches = new Dictionary<string, ConcurrentDictionary<string, Item>>();
        private static ConcurrentDictionary<string, Item> GetContentTokenCache(Database database)
        {
            ConcurrentDictionary<string, Item> cache;
            if (ContentTokenCaches.TryGetValue(database.Name, out cache))
            {
                return cache;
            }
            return ContentTokenCaches[database.Name] = new ConcurrentDictionary<string, Item>();
        }

        protected override string GetTextVariableValue(string variableKey)
        {
            if (!string.IsNullOrWhiteSpace(variableKey))
            {
                var contextDatabase = Sitecore.Context.Database;
                var cache = GetContentTokenCache(contextDatabase);
                Item variable;
                if (!cache.TryGetValue(variableKey, out variable))
                {
                    variable = contextDatabase.SelectSingleItem(
                        $"fast://*[@@templateid='{Templates.ContentToken.ID}' and @#Key#='{variableKey}']");
                    cache[variableKey] = variable;
                }

                if (variable != null)
                {
                    return variable[Templates.ContentToken.Fields.Value];
                }
            }
            return string.Empty;
        }

        [UsedImplicitly]
        private void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item item = Event.ExtractParameter(args, 0) as Item;
            if (item != null && item.TemplateID == Templates.ContentToken.ID)
            {
                Item result;
                GetContentTokenCache(item.Database).TryRemove(item["Key"], out result);
            }
        }

        [UsedImplicitly]
        private void OnItemSavedRemote(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            ItemSavedRemoteEventArgs itemSavedRemoteEventArgs = args as ItemSavedRemoteEventArgs;
            if (itemSavedRemoteEventArgs != null && itemSavedRemoteEventArgs.Item.TemplateID == Templates.ContentToken.ID)
            {
                Item result;
                GetContentTokenCache(itemSavedRemoteEventArgs.Item.Database).TryRemove(itemSavedRemoteEventArgs.Item["Key"], out result);
            }
        }

        [UsedImplicitly]
        private void OnItemDeleted(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item item = Event.ExtractParameter(args, 0) as Item;
            if (item != null && item.TemplateID == Templates.ContentToken.ID)
            {
                Item result;
                GetContentTokenCache(item.Database).TryRemove(item["Key"], out result);
            }
        }

        [UsedImplicitly]
        private void OnItemDeletedRemote(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            ItemDeletedRemoteEventArgs itemDeletedRemoteEventArgs = args as ItemDeletedRemoteEventArgs;
            if (itemDeletedRemoteEventArgs != null && itemDeletedRemoteEventArgs.Item.TemplateID == Templates.ContentToken.ID)
            {
                Item result;
                GetContentTokenCache(itemDeletedRemoteEventArgs.Item.Database).TryRemove(itemDeletedRemoteEventArgs.Item["Key"], out result);
            }
        }
    }
}