using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetStudio.Extensions;
using AssetStudio.Metas.Components;
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
            var SpPixelPivot = sprite.m_Pivot * new Vector2(sprite.m_Rect.width, sprite.m_Rect.height);
            var TexPixelPivot = SpPixelPivot - sprite.m_RD.textureRectOffset;
            pivot = TexPixelPivot / new Vector2(sprite.m_RD.textureRect.width, sprite.m_RD.textureRect.height);
            border = new Vector4 { X = sprite.m_Border.X, Y = sprite.m_Border.Y, Z = sprite.m_Border.Z, W = sprite.m_Border.W };
            alignment = (int)pivot.ToAlignment();
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
            var textureOffset = new Vector2(
                - m_Sprite.m_RD.textureRect.x + (m_Sprite.m_Rect.width - m_Sprite.m_RD.textureRect.width)/2,
                - m_Sprite.m_RD.textureRect.y + (m_Sprite.m_Rect.height - m_Sprite.m_RD.textureRect.height)/2
                );
            outline = new Vector2[triangles.Length][];
            for (int i = 0;i < triangles.Length;i++)
            {
                var triangle = triangles[i];
                outline[i] = new Vector2[3];
                for (int j = 0; j < triangle.Length;j++)
                {
                    outline[i][j] = triangle[j] * sprite.m_PixelsToUnits + pivotPixelsOffset + textureOffset; 
                }
            }

            var shape = sprite.m_PhysicsShape;
            physicsShape = new Vector2[shape.Length][];
            for (int i = 0; i < shape.Length;i++)
            {
                physicsShape[i] = new Vector2[shape[i].Length];
                for (int j = 0;j < shape[i].Length;j++)
                {
                    physicsShape[i][j] = shape[i][j] * sprite.m_PixelsToUnits + pivotPixelsOffset + textureOffset;
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

        private MCSprite m_MCSprite;
        public MCSpriteSheet(Sprite sprite)
        {
            serializedVersion = 2;
            spriteID = Guid.NewGuid().ToString("N");
            sprites = Array.Empty<MCSprite>();
            m_MCSprite = new MCSprite(sprite, 0);
            bones = m_MCSprite.bones;
            outline = m_MCSprite.outline;
            physicsShape = m_MCSprite.physicsShape;
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

        public Vector2 GetSpritePivot()
        {
            if (m_MCSprite != null)
            {
                return m_MCSprite.pivot;
            }
            if (sprites.Length > 0)
            {
                return sprites[0].pivot;
            }
            return Vector2.Zero;
        }

        public SpriteAlignment GetAlignment()
        {
            return GetSpritePivot().ToAlignment();
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

    public class TextureMeta : MetaObject
    {
        public TextureImporter TextureImporter;
        public TextureMeta(Sprite sprite)
        {
            TextureImporter = new TextureImporter(sprite);
        }
        public TextureMeta(Texture2D texture, SpriteAtlas spriteAtlas)
        {
            TextureImporter = new TextureImporter(texture, spriteAtlas);
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
