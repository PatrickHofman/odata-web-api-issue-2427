using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class CustomersController : ODataController
    {
        public CustomersController()
        { }

        [HttpGet]
        public IActionResult Get()
        {
            IEdmCollectionType collectionType;

            ODataPath oDataPath = Request.ODataFeature().Path;
            IEdmType edmType = oDataPath.Segments.First().EdmType;

            IEdmEntityType entityType;

            if (edmType is IEdmCollectionType ct)
            {
                collectionType = ct;
                entityType = collectionType.ElementType.Definition as IEdmEntityType;
            }
            else
            {
                throw new Exception($"Unexpected EDM type '{edmType?.GetType()}'.");
            }

            IEnumerable<IEdmEntityObject> entityObjects = this.GetData(entityType);

            IEdmCollectionTypeReference collectionTypeReference = new EdmCollectionTypeReference(collectionType);

            StreamingEdmEntityObjectCollection collection = new StreamingEdmEntityObjectCollection
                                                            ( collectionTypeReference
                                                            , entityObjects
                                                            );

            //
            // Return a collection type.
            //
            return Ok(collection);
        }

        private IEnumerable<IEdmEntityObject> GetData(IEdmEntityType entityType)
        {
            for (int id = 1; id <= 1_000_000; id++)
            {
                EdmEntityObject entity = new EdmEntityObject(entityType);

                entity.TrySetPropertyValue("CustomerId", id);
                entity.TrySetPropertyValue("Data", new byte[1000]);

                yield return entity;

                Trace.WriteLine($"Yielded {id:N0} rows.");
            }
        }
    }
}
