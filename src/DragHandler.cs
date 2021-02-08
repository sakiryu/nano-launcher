using CefSharp;
using CefSharp.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NanoLauncher
{
    public class DragHandler : IDragHandler
    {
        public Region DraggableRegion { get; set; } = new Region();
        public event Action<Region> _changedRegions;

        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask) =>
            false;

        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
            if (!browser.IsPopup)
            {
                if(!regions.Any())
                {
                    return;
                }

                (regions as List<DraggableRegion>).ForEach(r =>
                {
                    var rect = new Rectangle(r.X, r.Y, r.Width, r.Height);

                    if (DraggableRegion is null)
                    {
                        DraggableRegion = new Region(rect);
                    }
                    else
                    {
                        if (r.Draggable)
                        {
                            DraggableRegion.Union(rect);
                        }
                        else
                        {
                            DraggableRegion.Exclude(rect);
                        }
                    }
                });
                _changedRegions?.Invoke(DraggableRegion);
            }
        }

        public void Dispose() => _changedRegions = null;
    }
}
