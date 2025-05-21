namespace Volorf.GenImage
{
    public enum Model
    {
        GptImage1,
        DallE3,
        DallE2
    }
    
    public enum Provider
    {
        OpenAI
    }
    
    public enum FillMode
    {
        Stretch,
        PreserveAspect,
        None
    }
    
    public enum Quality
    {
        Low,
        Medium,
        High
    }
    
    public enum Size
    {
        Square,
        Landscape,
        Portrait
    }
}