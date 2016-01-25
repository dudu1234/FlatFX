using FlatFXResources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FlatFXWebClient.Controllers
{
    public class TranslationsController : ApiController
    {
        public IHttpActionResult Get(string lang)
        {
            var resourceObject = new JObject();

            var resourceSet = Resources.ResourceManager.GetResourceSet(new CultureInfo(lang), true, true);
            IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resourceObject.Add(enumerator.Key.ToString(), enumerator.Value.ToString());
            }

            return Ok(resourceObject);
        }
    }
}