namespace TalkRailwayProgramming._3_MakeExplicit;

public interface IExplicitDomain
{
    Task<Result<string, Error>> Run(int id);
}