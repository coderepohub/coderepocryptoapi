using AutoMapper;
using CryptoQuote.Models;

namespace CryptoQuote.Agent.Mappers
{
    public class CryptoCurrencyCodeMapperProfile : Profile
    {
        public CryptoCurrencyCodeMapperProfile()
        {
            CreateMap<CryptoCurrency, CryptoCurrencyCodeResponse>()
                .ForMember(d=>d.Code, o => o.MapFrom(s=>s.Symbol))
                .ForMember(d=>d.Name, o => o.MapFrom(s=>s.Name));
        }
    }
}
