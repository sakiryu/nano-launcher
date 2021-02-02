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
        public Region _draggableRegion;
        public event Action<Region> _changedRegions;

        public DragHandler() => _draggableRegion = new Region();

        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask) =>
            false;

        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
            if (!browser.IsPopup)
            {
                //_draggableRegion = null;

                (regions as List<DraggableRegion>).ForEach(r =>
                {
                    var rect = new Rectangle(r.X, r.Y, r.Width, r.Height);

                    if (_draggableRegion is null)
                    {
                        _draggableRegion = new Region(rect);
                    }
                    else
                    {
                        if (r.Draggable)
                        {
                            _draggableRegion.Union(rect);
                        }
                        else
                        {
                            _draggableRegion.Exclude(rect);
                        }
                    }
                });

               //if (regions.Count > 0)
               //{
               //    foreach (var region in regions)
               //    {
               //        var rect = new Rectangle(region.X, region.Y, region.Width, region.Height);
               //
               //        if (_draggableRegion is null)
               //        {
               //            _draggableRegion = new Region(rect);
               //        }
               //        else
               //        {
               //            if (region.Draggable)
               //            {
               //                _draggableRegion.Union(rect);
               //            }
               //            else
               //            {
               //                //In the scenario where we have an outer region, that is draggable and it has
               //                // an inner region that's not, we must exclude the non draggable.
               //                // Not all scenarios are covered in this example.
               //                _draggableRegion.Exclude(rect);
               //            }
               //        }
               //    }
               //}
                _changedRegions?.Invoke(_draggableRegion);
            }
        }

        public void Dispose() => _changedRegions = null;
    }
}
