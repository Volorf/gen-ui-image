namespace Volorf.GenImage
{
    public enum Model
    {
        DallE3,
        DallE2,
        GptImage1
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
        High,
        Auto
    }
    
    public enum Size
    {
        Square,
        Landscape,
        Portrait
    }
}