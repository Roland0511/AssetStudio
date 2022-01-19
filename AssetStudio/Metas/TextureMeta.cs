using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetStudio.Classes.Editor;
using AssetStudio.Extensions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace AssetStudio.Metas
{
    public struct MCBone
    {
        public string name;
        public Vector3 position;
        public Vector4 rotation;
        public float length;
        public int parentId;

    }
    public class MCRect : MetaComponent
    {
        private Rectf m_Rect;
        public float x => m_Rect.x;
        public float y => m_Rect.y;
        public float width => m_Rect.width;
        public float height => m_Rect.height;

        public MCRect(Rectf rect)
        {
            serializedVersion = 2;
            m_Rect = rect;
        }

    }

    public sealed class VectorConverter : IYamlTypeConverter
    {
        public IValueSerializer ValueSerializer { get; set; }
        public IValueDeserializer ValueDeserializer { get; set; }
        public bool Accepts(Type type)
        {
            return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            //parser.Consume<>
            return null;
        }

        //private void A

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Flow));
            if (type == typeof(Vector2))
            {
                var vector = (Vector2)value;

                ValueSerializer.SerializeValue(emitter, "x", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.X, typeof(float));
                ValueSerializer.SerializeValue(emitter, "y", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.Y, typeof(float));

            }
            else if (type == typeof(Vector3))
            {
                var vector = (Vector3)value;
                ValueSerializer.SerializeValue(emitter, "x", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.X, typeof(float));
                ValueSerializer.SerializeValue(emitter, "y", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.Y, typeof(float));
                ValueSerializer.SerializeValue(emitter, "z", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.Z, typeof(float));
            }
            else if (type == typeof(Vector4))
            {
                var vector = (Vector4)value;
                ValueSerializer.SerializeValue(emitter, "x", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.X, typeof(float));
                ValueSerializer.SerializeValue(emitter, "y", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.Y, typeof(float));
                ValueSerializer.SerializeValue(emitter, "z", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.Z, typeof(float));
                ValueSerializer.SerializeValue(emitter, "w", typeof(string));
                ValueSerializer.SerializeValue(emitter, vector.W, typeof(float));
            }

            emitter.Emit(new MappingEnd());
        }
    }

    public class MCSprite : MetaComponent
    {
        public string name;
        public MCRect rect;
        public int alignment;
        public Vector2 pivot;
        public Vector4 border;
        public uint internalId;
        public MCBone[] bones;
        public Vector2[][] outline;
        public Vector2[][] physicsShape;
        private Sprite m_Sprite;
        public MCSprite(Sprite sprite, uint id)
        {
            m_Sprite = sprite;
            
            serializedVersion = 2;
            internalId = id;
            name = sprite.m_Name;
            rect = new MCRect(sprite.GetRect());
            pivot = new Vector2 { X = sprite.m_Pivot.X, Y = sprite.m_Pivot.Y };
            border = new Vector4 { X = sprite.m_Border.X, Y = sprite.m_Border.Y, Z = sprite.m_Border.Z, W = sprite.m_Border.W };
            alignment = (int)sprite.GetAlignment();
            bones = new MCBone[sprite.m_Bones.Length];
            for (int i = 0; i < sprite.m_Bones.Length; i++)
            {
                var m_Bone = sprite.m_Bones[i];
                bones[i] = new MCBone
                {
                    name = m_Bone.name,
                    parentId = m_Bone.parentId,
                    position = BonePosUnitToPixel(m_Bone.position, sprite.m_Pivot, m_Bone.parentId == -1),
                    rotation = new Vector4 { X = m_Bone.rotation.X, Y = m_Bone.rotation.Y, Z = m_Bone.rotation.Z, W = m_Bone.rotation.W },
                    length = BoneLengthUnitToPixel(m_Bone.length),
                };
            }
            var triangles = sprite.GetTriangles();
            var pivotPixelsOffset = (sprite.m_Pivot - new Vector2(0.5f, 0.5f)) *
                new Vector2(sprite.m_Rect.width, sprite.m_Rect.height);
            outline = new Vector2[triangles.Length][];
            for (int i = 0;i < triangles.Length;i++)
            {
                var triangle = triangles[i];
                outline[i] = new Vector2[3];
                for (int j = 0; j < triangle.Length;j++)
                {
                    outline[i][j] = triangle[j] * sprite.m_PixelsToUnits + pivotPixelsOffset;
                }
            }

            var shape = sprite.m_PhysicsShape;
            physicsShape = new Vector2[shape.Length][];
            for (int i = 0; i < shape.Length;i++)
            {
                physicsShape[i] = new Vector2[shape[i].Length];
                for (int j = 0;j < shape[i].Length;j++)
                {
                    physicsShape[i][j] = shape[i][j] * sprite.m_PixelsToUnits + pivotPixelsOffset;
                }
            }
        }



        private float UnitValueToPixel(float unitValue, float pivotValue, float pixelsToUnits, float bandLength)
        {
            float offset = pivotValue * bandLength / pixelsToUnits;
            float pixelValue = (unitValue + offset) * pixelsToUnits;
            return pixelValue;
        }

        public float BoneLengthUnitToPixel(float unitLength)
        {
            return UnitValueToPixel(unitLength, 0, m_Sprite.m_PixelsToUnits, 0);
        }

        public Vector3 BonePosUnitToPixel(Vector3 unitPos, Vector2 unitPivot, bool isRoot)
        {
            Rectf rect = m_Sprite.GetRect();
            var x = UnitValueToPixel(unitPos.X, unitPivot.X, m_Sprite.m_PixelsToUnits, isRoot ? rect.width : 0);
            var y = UnitValueToPixel(unitPos.Y, unitPivot.Y, m_Sprite.m_PixelsToUnits, isRoot ? rect.height : 0);
            return new Vector3 { X = x, Y = y , Z = 0};
        }

    }

    public class MCSpriteSheet : MetaComponent
    {
        public MCSprite[] sprites;
        public MCBone[] bones;
        public Vector2[][] outline;
        public Vector2[][] physicsShape;

        public string spriteID;
        public MCSpriteSheet(Sprite sprite)
        {
            serializedVersion = 2;
            spriteID = Guid.NewGuid().ToString("N");
            sprites = Array.Empty<MCSprite>();
            var mcSprite = new MCSprite(sprite, 0);
            bones = mcSprite.bones;
            outline = mcSprite.outline;
            physicsShape = mcSprite.physicsShape;
        }

        public MCSpriteSheet(Sprite[] sprites, Dictionary<string, uint> internalIDDict)
        {
            serializedVersion = 2;
            this.sprites = new MCSprite[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                var sprite = sprites[i];
                this.sprites[i] = new MCSprite(sprite, internalIDDict[sprite.m_Name]);
            }
        }
        public MCSpriteSheet(PPtr<Sprite>[] spritePtrs, Dictionary<string, uint> internalIDDict)
        {
            serializedVersion = 2;
            sprites = new MCSprite[spritePtrs.Length];
            for (int i = 0; i < spritePtrs.Length; i++)
            {
                PPtr<Sprite> spritePtr = spritePtrs[i];
                if (spritePtr.TryGet(out Sprite sprite))
                {

                    sprites[i] = new MCSprite(sprite, internalIDDict[sprite.m_Name]);
                }

            }
        }
    }

    public struct MCInternalIDNamePair
    {
        public Dictionary<int, uint> first;
        public string second;
        public MCInternalIDNamePair(Dictionary<int, uint> key, string value)
        {
            first = key;
            second = value;
        }
    }


    public class MCTextureImporter : MetaComponent
    {
        public List<MCInternalIDNamePair> internalIDToNameTable;
        public MCSpriteSheet spriteSheet;
        public float spritePixelsToUnits;
        public Vector2 spritePivot;
        public Vector4 spriteBorder;
        public int alignment;
        public int textureType;
        public int spriteMode;
        public int alphaIsTransparency;
        public int nPOTScale;
        public MCTextureImporter(Sprite sprite)
        {
            serializedVersion = 11;
            textureType = (int)TextureImporterType.Sprite;
            spriteMode = (int)SpriteImportMode.Single;
            alphaIsTransparency = 1;
            alignment = (int)sprite.GetAlignment();
            nPOTScale = (int)TextureImporterNPOTScale.None;
            spritePixelsToUnits = sprite.m_PixelsToUnits;
            spritePivot = sprite.m_Pivot;
            spriteBorder = sprite.m_Border;
            spriteSheet = new MCSpriteSheet(sprite);
            internalIDToNameTable = new List<MCInternalIDNamePair>();
        }

        public MCTextureImporter(Texture2D texture, SpriteAtlas spriteAtlas)
        {
            serializedVersion = 11;
            textureType = (int)TextureImporterType.Sprite;
            spriteMode = (int)SpriteImportMode.Multiple;
            alphaIsTransparency = 1;

            var matchedSprites = new List<PPtr<Sprite>>();
            var sprites = spriteAtlas.m_PackedSprites;
            internalIDToNameTable = new List<MCInternalIDNamePair>();
            var internalIDDict = new Dictionary<string, uint>();
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].TryGet(out Sprite sprite))
                {
                    var spTexture2D = sprite.GetTexture();
                    if (spTexture2D != null && spTexture2D.m_Name == texture.m_Name)
                    {
                        if(spritePixelsToUnits == 0)spritePixelsToUnits = sprite.m_PixelsToUnits;
                        matchedSprites.Add(sprites[i]);
                        var classId = (int)ClassIDType.Sprite;
                        var internalId = (int)ClassIDType.Sprite * 100000 + i * 2;
                        var internalKey = new Dictionary<int, uint>
                        {
                            [classId] = (uint)internalId
                        };
                        internalIDToNameTable.Add(new MCInternalIDNamePair(internalKey, sprite.m_Name));
                        internalIDDict[sprite.m_Name] = (uint)internalId;
                    }
                }
            }
            alignment = 0;
            spriteSheet = new MCSpriteSheet(matchedSprites.ToArray(), internalIDDict);
        }
    }
    public class TextureMeta : MetaObject
    {
        public MCTextureImporter TextureImporter;
        public TextureMeta(Sprite sprite)
        {
            TextureImporter = new MCTextureImporter(sprite);
        }
        public TextureMeta(Texture2D texture, SpriteAtlas spriteAtlas)
        {
            TextureImporter = new MCTextureImporter(texture, spriteAtlas);
        }

        protected override SerializerBuilder CreateSerializerBuilder()
        {
            return CreateMetaSerializerBuilder();
        }

        public static SerializerBuilder CreateMetaSerializerBuilder()
        {
            var vectorConverter = new VectorConverter();
            var builder = new SerializerBuilder().WithTypeConverter(vectorConverter);
            vectorConverter.ValueSerializer = builder.BuildValueSerializer();
            return builder;
        }
    }
}
