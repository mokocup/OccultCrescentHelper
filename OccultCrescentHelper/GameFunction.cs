using Dalamud.Utility.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccultCrescentHelper
{
    internal class GameFunction
    {
        private delegate nint GetPublicContentOccultCrescentInstanceDelegate();

        // Waiting CS Update from Dalamud
        [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 08 0F B6 CB")]
        private readonly GetPublicContentOccultCrescentInstanceDelegate? _getPublicContentOccultCrescentInstance = null;

        internal GameFunction()
        {
            OccultCrescentHelper.GameInteropProvider.InitializeFromAttributes(this);
        }

        public nint GetPublicContentOccultCrescentInstance()
        {
            if (this._getPublicContentOccultCrescentInstance == null)
                throw new InvalidOperationException("GetPublicContentOccultCrescentInstance signature wasn't found!");

            return this._getPublicContentOccultCrescentInstance();
        }
    }
}
