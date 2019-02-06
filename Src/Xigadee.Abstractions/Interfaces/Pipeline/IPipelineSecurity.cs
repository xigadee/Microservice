namespace Xigadee
{
    /// <summary>
    /// This is the generic channel interface
    /// </summary>
    public interface IPipelineSecurity<out P>: IPipelineSecurity, IPipelineExtension<P>
        where P : IPipeline
    {
    }

    public interface IPipelineSecurity
    {

    }

}
