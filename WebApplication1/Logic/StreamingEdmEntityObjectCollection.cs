using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;
using System.Collections;
using System.Collections.Generic;

namespace WebApplication1
{
    /// <summary>
    /// Represents an <see cref="IEdmObject"/> that is a collection of <see cref="IEdmEntityObject"/>s.
    /// </summary>
    //[NonValidatingParameterBinding]
    public class StreamingEdmEntityObjectCollection : IEnumerable<IEdmEntityObject>, IEdmObject
    {
        private IEdmCollectionTypeReference _edmType;

        private IEnumerator<IEdmEntityObject> listEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityObjectCollection"/> class.
        /// </summary>
        /// <param name="edmType">The EDM type of the collection.</param>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        public StreamingEdmEntityObjectCollection(IEdmCollectionTypeReference edmType, IEnumerable<IEdmEntityObject> list)
        {
            this._edmType = edmType;

            //
            // Prefetch first entity to let exceptions bubble up early.
            //
            this.listEnumerator = list.GetEnumerator();
        }

        /// <inheritdoc/>
        public IEdmTypeReference GetEdmType()
        {
            return this._edmType;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IEdmEntityObject> GetEnumerator()
        {
            while (this.listEnumerator.MoveNext())
            {
                yield return this.listEnumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
