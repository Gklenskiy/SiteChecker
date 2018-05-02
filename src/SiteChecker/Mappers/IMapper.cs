namespace SiteChecker.Mappers
{
	public interface IMapper<in T1, T2>
	{
		T2 Map(T2 result, T1 source);
	}
}