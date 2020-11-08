﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content.Runtime.Graphics;

namespace Microsoft.Xna.Framework.Graphics
{
    public interface IMeshGeometry
    {
        void Bind(GraphicsDevice device, bool isMirrorTransform);
        void Draw(GraphicsDevice device);
    }

    public class MeshTriangles : IMeshGeometry
    {
        #region lifecycle

        public static MeshTriangles CreateFrom(MeshGeometryContent content, IReadOnlyList<VertexBuffer> vb, IReadOnlyList<IndexBuffer> ib)
        {
            var tris = new MeshTriangles();

            tris.SetVertexBuffer(vb[content.VertexBufferIndex], content.VertexOffset, content.VertexCount);
            tris.SetIndexBuffer(ib[content.IndexBufferIndex], content.IndexOffset, content.PrimitiveCount);
            tris.SetCullingStates(false);

            return tris;
        }

        public void SetVertexBuffer(VertexBuffer vb, int offset, int count)
        {
            this._SharedVertexBuffer = vb;
            this._VertexOffset = offset;
            this._VertexCount = count;
        }

        public void SetIndexBuffer(IndexBuffer ib, int offset, int count)
        {
            this._SharedIndexBuffer = ib;
            this._IndexOffset = offset;
            this._PrimitiveCount = count;
        }

        public void SetCullingStates(bool doubleSided)
        {
            FrontRasterizer = doubleSided ? RasterizerState.CullNone : RasterizerState.CullCounterClockwise;
            BackRasterizer = doubleSided ? RasterizerState.CullNone : RasterizerState.CullClockwise;
        }

        #endregion

        #region data

        // state used for normal rendering
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private RasterizerState _FrontRasterizer = RasterizerState.CullCounterClockwise;

        // state used for mirrored tranform. This must be the same as _FrontRasterizer with reversed CullMode.
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private RasterizerState _BackRasterizer = RasterizerState.CullClockwise;
        
        private IndexBuffer _SharedIndexBuffer;        
        private int _IndexOffset;        
        private int _PrimitiveCount;
        
        private VertexBuffer _SharedVertexBuffer;        
        private int _VertexOffset;        
        private int _VertexCount;

        #endregion

        #region properties        

        public RasterizerState FrontRasterizer
        {
            get => _FrontRasterizer;
            set => _FrontRasterizer = value;
        }

        public RasterizerState BackRasterizer
        {
            get => _BackRasterizer;
            set => _BackRasterizer = value;
        }
        
        #endregion

        #region API

        public void Bind(GraphicsDevice device, bool isMirrorTransform)
        {
            device.SetVertexBuffer(_SharedVertexBuffer);
            device.Indices = _SharedIndexBuffer;

            device.RasterizerState = isMirrorTransform ? _BackRasterizer : _FrontRasterizer;
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, _VertexOffset, _IndexOffset, _PrimitiveCount);            
        }

        public TVertex[] TryGetVertices<TVertex>()
            where TVertex:struct,IVertexType
        {
            var data = new TVertex[_VertexCount];
            _SharedVertexBuffer.GetData<TVertex>(data, _VertexOffset, _VertexCount);
            return data;
        }
        
        #endregion
    }

    public class MeshLines : IMeshGeometry
    {
        #region lifecycle       

        public void SetVertexBuffer(VertexBuffer vb, int offset, int count)
        {
            this._SharedVertexBuffer = vb;
            this._VertexOffset = offset;
            this._VertexCount = count;
        }

        public void SetIndexBuffer(IndexBuffer ib, int offset, int count)
        {
            this._SharedIndexBuffer = ib;
            this._IndexOffset = offset;
            this._PrimitiveCount = count;
        }

        #endregion

        #region data

        // state used for line rendering
        private RasterizerState _LineRasterizer = RasterizerState.CullNone;

        private IndexBuffer _SharedIndexBuffer;
        private int _IndexOffset;
        private int _PrimitiveCount;

        private VertexBuffer _SharedVertexBuffer;
        private int _VertexOffset;
        private int _VertexCount;

        #endregion

        #region properties        

        public RasterizerState LineRasterizer
        {
            get => _LineRasterizer;
            set => _LineRasterizer = value;
        }
        
        #endregion

        #region API

        public void Bind(GraphicsDevice device, bool isMirrorTransform)
        {
            device.SetVertexBuffer(_SharedVertexBuffer);
            device.Indices = _SharedIndexBuffer;

            device.RasterizerState = _LineRasterizer;
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, _VertexOffset, _IndexOffset, _PrimitiveCount);
        }

        #endregion
    }
}
