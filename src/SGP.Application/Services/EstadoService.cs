using AutoMapper;
using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using SGP.Application.Interfaces;
using SGP.Application.Responses;
using SGP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGP.Application.Services
{
    public class EstadoService : IEstadoService
    {
        private const string ObterTodosCacheKey = "EstadoService__ObterTodosAsync";
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IEstadoRepository _repository;

        public EstadoService(IMapper mapper, IMemoryCache memoryCache, IEstadoRepository repository)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _repository = repository;
        }

        public async Task<Result<IEnumerable<EstadoResponse>>> ObterTodosAsync()
        {
            return await _memoryCache.GetOrCreateAsync(ObterTodosCacheKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(60);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);

                var estados = await _repository.ObterTodosAsync();
                return Result.Ok(_mapper.Map<IEnumerable<EstadoResponse>>(estados));
            });
        }
    }
}