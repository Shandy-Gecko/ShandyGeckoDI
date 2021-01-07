namespace ShandyGecko.ShandyGeckoDI
{
	public interface ILifeTimeCheckerReactions
	{
		LifeTimeReaction NullContextReaction { get; set; }
		LifeTimeReaction LifeTimeErrorReaction { get; set; }
	}
}