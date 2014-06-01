using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Windows.ApplicationModel.Resources.Core;

namespace PwdHash.WinStore.Common
{
    class ResWrapper
    {
        public static Dictionary<string, string> Strings { get; private set; }

        public ResWrapper()
        {
            loadResources();
        }

        private void loadResources()
        {
            var res = Windows.ApplicationModel.Resources.Core.ResourceManager.Current;
            Strings = toDictionary(res.MainResourceMap.GetSubtree("Resources"));
        }

        private Dictionary<string, string> toDictionary(ResourceMap resourceMap)
        {
            if (resourceMap == null)
                return null;

            var strings = resourceMap.ToDictionary(p => p.Key, p =>
            {
                try
                {
                    return resourceMap.GetValue(p.Key).ValueAsString;
                }
                catch (Exception e)
                {
                    //var ex = new ResWrapperException(String.Format("Resource key missing: {0}", p.Key));
                    //ServiceLocator.Current.GetInstance<IBugSenseLogger>().LogException(ex, "ResWrapper", p.Key);
                    //throw ex;
                    return null;
                }
            }
            );
            return strings;
        }
    }
}
