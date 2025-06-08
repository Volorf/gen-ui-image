# Gen UI Image
Create a UI image with AI-generated content in Unity.

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/generating-preview.gif" width="800" alt="Generate a preview placeholder for a product card">


## System Requirements
`Unity 2022.3` or later.

## Installation
1. Copy `Git URL`;
2. In Unity, open `Window > Package Manager`;
3. Click on the `+` icon in the top left corner and select `Add package from git URL`;

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/gen-ui-image__installation__1280w__10fps.gif" width="800" alt="How to install Gen UI Image in Unity3D">

## AI Providers
Right now, `Gen UI Image` supports only **OpenAI** API. 

**Available OpenAI Models:**
* `dall-e-2`
* `dall-e-3`
* `gpt-image-1`

> [!IMPORTANT]
> To work with `gpt-image-1` model, your organization must be verified. For me, it took a few minutes. More about this [here](https://help.openai.com/en/articles/10910291-api-organization-verification).

## How to get `API Key`

`GenUIImage` needs your `API Key` to get access to OpenAI generative models. To get the `API key`:
1. Go to your [OpenAI Account](https://platform.openai.com/settings/) 
2. In `Settings` find `API Keys`. 
3. Click `Create new secret key` and set it up. Copy it.

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/gen-ui-image__getting-api-key__openai.png" width="800" alt="How to get OpenAI Secret Key">

More information you can find on [OpenAI's official guide](https://help.openai.com/en/articles/4936850-where-do-i-find-my-openai-api-key).

> [!IMPORTANT]
> If you plan to share your build with someone you do not know, you must consider a way to secure how you store your **API keys**. In runtime, you can call `Generate(string apiKey)` method on a `GenUIImage` instance to explicitly pass your securely obtained key. Otherwise, it will try to extract it from `PlayerPrefs`.

## How to add `API Key` to `Gen UI Image`

1. In Unity, open `Tools > Gen UI Image > Settings`.
2. Paste your key in the field.
3. Save it.

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/gen-ai-image__adding-api-key_1280w_10fps.gif" width="800" alt="How to add OpenAI Secret Key to Gen UI Image">

## How to create `Gen UI Image`
1. Select `Canvas` and `Right Mouse Click` to call the context menu.
2. Select `UI > Gen UI Image`.
3. Enter your prompt and click `Generate`.

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/gen-ai-image__adding-gen-ui-image__1280w__10fps.gif" width="800" alt="How to create Gen UI Image">

> [!TIP]
> If you try to send an empty prompt you will get a cute red panda eating an apple.

## How to save a generated image
After an image is generated, you will see a `Save as Asset` button. Click and the image will be saved there `Assets/Volorf/Gen UI Image/Generated Images`.

<img src="https://www.olegfrolov.design/images/other/gen-ui-image/gen-ai-image__saving-as-asset__1280w__10fps.gif" width="800" alt="How to save a generated image">

## How to use `Gen UI Image` in builds
By default `API Key` is stored locally on your computer with `PlayerPrefs`. To carry on this information to your builds you need to do following:
1. Add a `ApiKeysProvider` component to your `Gen UI Image`.
2. Open the context menu and select `Create > Gen UI Image > Create ApiKeysProviderData`. It will create a data asset.
3. Add your `API key` to the asset.
4. Drag and drop the asset to the `ApiKeysProvider` component.

> [!IMPORTANT]
> If you plan to give the build to someone you do not know, you must consider explicitly passing your securely obtained key to `Generate(string apiKey)` on your `GenUIImage` instance.

## How to update `Gen UI Image` in runtime
Just call the `Generate()` method in your `GenUIImage` instance.
You can also specify some of the parameters before the call.
Here is an example:

```csharp
_genUiImage.model = Model.GptImage1;
_genUiImage.size = Size.Portrait;
_genUiImage.quality = Quality.High;
_genUiImage.prompt = "A cute red panda eating crunchy biscuits.";
_genUiImage.Generate();
```

> [!NOTE]
> If you just to the `Generate()` method without specifying anything, it will use the serialized values that were set up in your editor.

For security reasons you might want to explicitly pass `API Key` as a parameter in the `Generate()` method. Example:

```csharp
// The most secure way to store the keys is to use a server
var keyObtainer = new MySecureCloudApiKeyObtainer();
var key = keyObtainer.GetOpenAiKey();

_genUiImage.Generate(apiKey: key);
```

## How to get just `Texture`
### Option 1
If you want to get just `Texture` you could get it via your `GenUIImage` instance by using `Texture` property (you need to make sure that you called `Generate()` before!).

```csharp
_genUiImage.Generate();
var myTexture = _genUiImage.Texture;
```

### Option 2
You can create an instance of `GenRequestManager` class and call `GenerateTexture2D` method. In this case, you might need to pass your `API Key` as a parameter.

```csharp
public async Task<Texture2D> DummyGenerateTexture2D(
    Provider provider,
    Model model,
    Quality quality,
    Size size,
    string prompt,
    string apiKey = "")
{
     // Handles your request
     // and returns a texture
}
```

> [!NOTE]
> The method is `async`. Make sure that you handle it appropriately.

```csharp
GenRequestManager _genRequestManager = new GenRequestManager();
Texture myTexture = await _genRequestManager.GenerateTexture2D(
    provider: Provider.OpenAI,
    model: Model.GptImage1,
    quality: Quality.High,
    size: Size.Portrait,
    prompt: "A red panda climbing a tree and eating bamboo.",
    apiKey: "SecretApiKeyFromYourAiProvider"
)
```

## Contact me
[X](https://www.x.com/volorf) | [Bsky](https://bsky.app/profile/volorf.bsky.social) | [Linkedin](https://www.linkedin.com/in/olegfrolovdesign/) | [Personal Site](https://olegfrolov.design/)