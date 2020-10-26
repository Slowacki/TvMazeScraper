using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TvMazeScraper.Models;
using TvMazeScraper.Repositories;
using TvMazeScraper.Services;
using TvMazeScraper.Web.Configuration;

namespace TvMazeScraper.Web.Services
{
    public class ShowsService : IShowsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IShowsRepository _showsRepository;
        private readonly ClientNamesOptions _clientNamesOptions;
        private readonly UrlsOptions _urlsOptions;

        public ShowsService(IShowsRepository showsRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<ClientNamesOptions> applicationOptions,
            IOptions<UrlsOptions> urlsOptions)
        {
            _showsRepository = showsRepository;
            _httpClientFactory = httpClientFactory;
            _clientNamesOptions = applicationOptions.Value;
            _urlsOptions = urlsOptions.Value;
        }

        public async Task<IEnumerable<Show>> GetAsync(int page = 0)
        {
            var existingShows = await _showsRepository.GetAsync(page);
            if (existingShows.Any())
                return existingShows;

            var shows = await GetShowsDataOnlineAsync(page);

            await EnrichShowsWithCastAsync(shows);
            await _showsRepository.InsertAsync(shows);

            return shows;
        }

        private async Task EnrichShowsWithCastAsync(IEnumerable<Show> shows)
        {
            var client = _httpClientFactory.CreateClient(_clientNamesOptions.Shows);
            var createdDate = DateTime.Now;

            foreach (var show in shows)
            {
                var castResponse = await client.GetAsync($"{_urlsOptions.Shows}/{show.Id}/cast");

                if (castResponse.IsSuccessStatusCode)
                {
                    var cast = await castResponse.Content.ReadAsAsync<List<CastMember>>();
                    show.Cast = cast.Select(c => c.Person).OrderByDescending(o => o.Birthday).ToList();
                    show.CreatedAt = createdDate;
                }
            }
        }

        private async Task<IEnumerable<Show>> GetShowsDataOnlineAsync(int page = 0)
        {
            var client = _httpClientFactory.CreateClient(_clientNamesOptions.Cast);

            var response = await client.GetAsync($"{_urlsOptions.Shows}?page={page}");
            List<Show> shows;

            if (response.IsSuccessStatusCode)
            {
                shows = await response.Content.ReadAsAsync<List<Show>>();
            }
            else
            {
                throw new HttpRequestException(
                    $"Could not retrieve data from the online service. Error code: {response.StatusCode}");
            }

            return shows;
        }
    }
}