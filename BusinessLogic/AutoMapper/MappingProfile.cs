using AutoMapper;
using Models;
using Models.ResponseModels;

namespace BusinessLogic.AutoMapper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			this.CreateMap<Post, PostResponseModel>().ForMember(dest => dest.UpCount, opt => opt.MapFrom(src => src.Ups));
			this.CreateMap<UserPostCount, UserResponseModel>();
        }
	}
}

