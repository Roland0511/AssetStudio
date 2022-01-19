using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudio.Classes.Editor
{
    public class TextureImporter
    {
    }
    public enum TextureImporterType
    {
        /// <summary>
        ///   <para>This is the most common setting used for all the textures in general.</para>
        /// </summary>
        Default = 0,
        /// <summary>
        ///   <para>This is the most common setting used for all the textures in general.</para>
        /// </summary>
        [Obsolete("Use Default (UnityUpgradable) -> Default")] Image = 0,
        [Obsolete("Use NormalMap (UnityUpgradable) -> NormalMap")] Bump = 1,
        /// <summary>
        ///   <para>Select this to turn the color channels into a format suitable for real-time normal mapping.</para>
        /// </summary>
        NormalMap = 1,
        /// <summary>
        ///   <para>Use this if your texture is going to be used on any HUD/GUI Controls.</para>
        /// </summary>
        GUI = 2,
        [Obsolete("Use importer.textureShape = TextureImporterShape.TextureCube")] Cubemap = 3,
        [Obsolete("Use a texture setup as a cubemap with glossy reflection instead")] Reflection = 3,
        /// <summary>
        ///   <para>This sets up your texture with the basic parameters used for the Cookies of your lights.</para>
        /// </summary>
        Cookie = 4,
        [Obsolete("Use Default instead. All texture types now have an Advanced foldout (UnityUpgradable) -> Default")] Advanced = 5,
        /// <summary>
        ///   <para>This sets up your texture with the parameters used by the lightmap.</para>
        /// </summary>
        Lightmap = 6,
        /// <summary>
        ///   <para>Use this if your texture is going to be used as a cursor.</para>
        /// </summary>
        Cursor = 7,
        /// <summary>
        ///   <para>Select this if you will be using your texture for Sprite graphics.</para>
        /// </summary>
        Sprite = 8,
        [Obsolete("HDRI is not supported anymore")] HDRI = 9,
        /// <summary>
        ///   <para>Use this for texture containing a single channel.</para>
        /// </summary>
        SingleChannel = 10, // 0x0000000A
        /// <summary>
        ///   <para>Use this for textures that contain shadowmask data.</para>
        /// </summary>
        Shadowmask = 11, // 0x0000000B
        /// <summary>
        ///   <para>Use this for textures that contain directional lightmap data.</para>
        /// </summary>
        DirectionalLightmap = 12, // 0x0000000C
    }

    /// <summary>
    ///   <para>Texture importer modes for Sprite import.</para>
    /// </summary>
    public enum SpriteImportMode
    {
        /// <summary>
        ///   <para>Graphic is not a Sprite.</para>
        /// </summary>
        None,
        /// <summary>
        ///   <para>Sprite is a single image section extracted automatically from the texture.</para>
        /// </summary>
        Single,
        /// <summary>
        ///   <para>Sprites are multiple image sections extracted from the texture.</para>
        /// </summary>
        Multiple,
        /// <summary>
        ///   <para>Sprite has it own mesh outline defined.</para>
        /// </summary>
        Polygon,
    }

    /// <summary>
    ///   <para>Scaling mode for non power of two textures in TextureImporter.</para>
    /// </summary>
    public enum TextureImporterNPOTScale
    {
        /// <summary>
        ///   <para>Keep non power of two textures as is.</para>
        /// </summary>
        None,
        /// <summary>
        ///   <para>Scale to nearest power of two.</para>
        /// </summary>
        ToNearest,
        /// <summary>
        ///   <para>Scale to larger power of two.</para>
        /// </summary>
        ToLarger,
        /// <summary>
        ///   <para>Scale to smaller power of two.</para>
        /// </summary>
        ToSmaller,
    }


    /// <summary>
    ///   <para>How a Sprite's graphic rectangle is aligned with its pivot point.</para>
    /// </summary>
    public enum SpriteAlignment
    {
        /// <summary>
        ///   <para>Pivot is at the center of the graphic rectangle.</para>
        /// </summary>
        Center,
        /// <summary>
        ///   <para>Pivot is at the top left corner of the graphic rectangle.</para>
        /// </summary>
        TopLeft,
        /// <summary>
        ///   <para>Pivot is at the center of the top edge of the graphic rectangle.</para>
        /// </summary>
        TopCenter,
        /// <summary>
        ///   <para>Pivot is at the top right corner of the graphic rectangle.</para>
        /// </summary>
        TopRight,
        /// <summary>
        ///   <para>Pivot is at the center of the left edge of the graphic rectangle.</para>
        /// </summary>
        LeftCenter,
        /// <summary>
        ///   <para>Pivot is at the center of the right edge of the graphic rectangle.</para>
        /// </summary>
        RightCenter,
        /// <summary>
        ///   <para>Pivot is at the bottom left corner of the graphic rectangle.</para>
        /// </summary>
        BottomLeft,
        /// <summary>
        ///   <para>Pivot is at the center of the bottom edge of the graphic rectangle.</para>
        /// </summary>
        BottomCenter,
        /// <summary>
        ///   <para>Pivot is at the bottom right corner of the graphic rectangle.</para>
        /// </summary>
        BottomRight,
        /// <summary>
        ///   <para>Pivot is at a custom position within the graphic rectangle.</para>
        /// </summary>
        Custom,
    }
}
