using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class provides shared functionality for search results.
    /// </summary>
    public static class SearchResponseHelper
    {
        /// <summary>
        /// This class provides shared functionality to convert a search result in to an equivalent search result
        /// with a model replacement to send out from the API.
        /// </summary>
        /// <typeparam name="E">The entity.</typeparam>
        /// <typeparam name="M">The model.</typeparam>
        /// <param name="response">The incoming search request entity object.</param>
        /// <param name="converter">The model converter.</param>
        /// <param name="skip">The optional entity skip function.</param>
        /// <param name="adjustSkippedTotal">Specifies whether the total record count should be adjusted for skipped records.</param>
        /// <param name="adjust">This action can be used to adjust each model individually.</param>
        /// <returns>Returns the relevant response.</returns>
        public static SearchResponse<M> ConvertToModel<E, M>(this SearchResponse<E> response, Func<E, M> converter, Func<E, bool> skip = null, bool adjustSkippedTotal = true, Action<E,M> adjust = null)
        {
            var modelResponse = new SearchResponse<M>();

            modelResponse.Etag = response.Etag;
            modelResponse.Top = response.Top;
            modelResponse.Skip = response.Skip;
            modelResponse.TotalRecordCount = response.TotalRecordCount;

            //Convert the entities in to the model format.
            foreach (var entity in response.Data)
            {
                if (skip != null && skip(entity))
                {
                    if (adjustSkippedTotal && (modelResponse.TotalRecordCount ?? 0) > 0)
                        modelResponse.TotalRecordCount = modelResponse.TotalRecordCount.Value - 1;

                    continue;
                }

                var model = converter(entity);
                adjust?.Invoke(entity,model);
                modelResponse.Data.Add(model);
            }

            return modelResponse;
        }

        /// <summary>
        /// This class provides shared functionality to convert a search result in to an equivalent search result
        /// with a model replacement to send out from the API. This method uses async functions to allow for repository calls in the model.
        /// </summary>
        /// <typeparam name="E">The entity.</typeparam>
        /// <typeparam name="M">The model.</typeparam>
        /// <param name="response">The incoming search request entity object.</param>
        /// <param name="converter">The model converter.</param>
        /// <param name="skip">The optional entity skip function.</param>
        /// <param name="adjustSkippedTotal">Specifies whether the total record count should be adjusted for skipped records.</param>
        /// <param name="adjust">This action can be used to adjust each model individually.</param>
        /// <returns>Returns the relevant response.</returns>
        public static async Task<SearchResponse<M>> ConvertToModelAsync<E, M>(this SearchResponse<E> response, Func<E, Task<M>> converter, Func<E, Task<bool>> skip = null, bool adjustSkippedTotal = true, Action<E, M> adjust = null)
        {
            var modelResponse = new SearchResponse<M>();

            modelResponse.Etag = response.Etag;
            modelResponse.Top = response.Top;
            modelResponse.Skip = response.Skip;
            modelResponse.TotalRecordCount = response.TotalRecordCount;

            //Convert the entities in to the model format.
            foreach (var entity in response.Data)
            {
                if (skip != null && await skip(entity))
                {
                    if (adjustSkippedTotal && (modelResponse.TotalRecordCount ?? 0) > 0)
                        modelResponse.TotalRecordCount = modelResponse.TotalRecordCount.Value - 1;

                    continue;
                }

                var model = await converter(entity);
                adjust?.Invoke(entity, model);
                modelResponse.Data.Add(model);
            }

            return modelResponse;
        }
    }
}
