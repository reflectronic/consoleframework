﻿using ConsoleFramework.Core;
using ConsoleFramework.Events;
using ConsoleFramework.Native;

namespace ConsoleFramework.Controls
{
    public class Button : Control {

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Button));

        public event RoutedEventHandler OnClick {
            add {
                AddHandler(ClickEvent, value);
            }
            remove {
                RemoveHandler(ClickEvent, value);
            }
        }

        public Button() {
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(Button_OnMouseDown));
            AddHandler(MouseUpEvent, new MouseButtonEventHandler(Button_OnMouseUp));
            AddHandler(MouseEnterEvent, new MouseEventHandler(Button_MouseEnter));
            AddHandler(MouseLeaveEvent, new MouseEventHandler(Button_MouseLeave));
        }

        private string caption;
        public string Caption {
            get {
                return caption;
            }
            set {
                if (caption != value) {
                    caption = value;
                }
            }
        }

        private bool clicking;
        private bool showPressed;

        protected override Size MeasureOverride(Size availableSize) {
            Size minButtonSize = new Size(caption.Length + 4, 2);
            return minButtonSize;
            //return new Size(Math.Max(minButtonSize.width, availableSize.width - 1), Math.Max(minButtonSize.height, availableSize.height - 1));
        }
        
        public override void Render(RenderingBuffer buffer) {
            ushort captionAttrs = Color.Attr(Color.Black, Color.DarkGreen);
            if (showPressed) {
                buffer.SetPixel(0, 0, ' ');
                buffer.SetOpacity(0, 0, 3);
                buffer.FillRectangle(1, 0, ActualWidth - 1, 1, ' ', captionAttrs);
                if (!string.IsNullOrEmpty(Caption)) {
                    int titleStartX = 3;
                    bool renderTitle = false;
                    string renderTitleString = null;
                    int availablePixelsCount = ActualWidth - titleStartX * 2;
                    if (availablePixelsCount > 0) {
                        renderTitle = true;
                        if (Caption.Length <= availablePixelsCount) {
                            // dont truncate title
                            titleStartX += (availablePixelsCount - Caption.Length) / 2;
                            renderTitleString = Caption;
                        } else {
                            renderTitleString = Caption.Substring(0, availablePixelsCount);
                            if (renderTitleString.Length > 2) {
                                renderTitleString = renderTitleString.Substring(0, renderTitleString.Length - 2) + "..";
                            } else {
                                renderTitle = false;
                            }
                        }
                    }
                    if (renderTitle) {
                        // assert !string.IsNullOrEmpty(renderingTitleString);
                        buffer.SetPixel(titleStartX - 1, 0, ' ', (CHAR_ATTRIBUTES)captionAttrs);
                        for (int i = 0; i < renderTitleString.Length; i++) {
                            buffer.SetPixel(titleStartX + i, 0, renderTitleString[i], (CHAR_ATTRIBUTES)captionAttrs);
                        }
                        buffer.SetPixel(titleStartX + renderTitleString.Length, 0, ' ', (CHAR_ATTRIBUTES)captionAttrs);
                    }
                }
                buffer.SetPixel(0, 1, ' ');
                buffer.SetOpacityRect(0, 1, ActualWidth, 1, 3);
                buffer.FillRectangle(0, 1, ActualWidth - 1, 1, ' ', CHAR_ATTRIBUTES.NO_ATTRIBUTES);
                buffer.DumpOpacityMatrix();
            } else {
                buffer.FillRectangle(0, 0, ActualWidth - 1, 1, ' ', captionAttrs);
                if (!string.IsNullOrEmpty(Caption)) {
                    int titleStartX = 2;
                    bool renderTitle = false;
                    string renderTitleString = null;
                    int availablePixelsCount = ActualWidth - titleStartX*2;
                    if (availablePixelsCount > 0) {
                        renderTitle = true;
                        if (Caption.Length <= availablePixelsCount) {
                            // dont truncate title
                            titleStartX += (availablePixelsCount - Caption.Length) / 2;
                            renderTitleString = Caption;
                        } else {
                            renderTitleString = Caption.Substring(0, availablePixelsCount);
                            if (renderTitleString.Length > 2) {
                                renderTitleString = renderTitleString.Substring(0, renderTitleString.Length - 2) + "..";
                            } else {
                                renderTitle = false;
                            }
                        }
                    }
                    if (renderTitle) {
                        // assert !string.IsNullOrEmpty(renderingTitleString);
                        buffer.SetPixel(titleStartX - 1, 0, ' ', (CHAR_ATTRIBUTES)captionAttrs);
                        for (int i = 0; i < renderTitleString.Length; i++) {
                            buffer.SetPixel(titleStartX + i, 0, renderTitleString[i], (CHAR_ATTRIBUTES)captionAttrs);
                        }
                        buffer.SetPixel(titleStartX + renderTitleString.Length, 0, ' ', (CHAR_ATTRIBUTES)captionAttrs);
                    }
                }
                buffer.SetPixel(0, 1, ' ');
                buffer.SetOpacityRect(0, 1, ActualWidth, 1, 3);
                buffer.FillRectangle(1, 1, ActualWidth - 1, 1, '\u2580', CHAR_ATTRIBUTES.NO_ATTRIBUTES);
                buffer.SetOpacity(ActualWidth - 1, 0, 3);
                buffer.SetPixel(ActualWidth - 1, 0, '\u2584');
                buffer.DumpOpacityMatrix();
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs args) {
            if (clicking) {
                if (!showPressed) {
                    showPressed = true;
                    Invalidate();
                }
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs args) {
            if (clicking) {
                if (showPressed) {
                    showPressed = false;
                    Invalidate();
                }
            }
        }

        public void Button_OnMouseDown(object sender, MouseButtonEventArgs args) {
            if (!clicking) {
                clicking = true;
                showPressed = true;
                ConsoleApplication.Instance.BeginCaptureInput(this);
                this.Invalidate();
                args.Handled = true;
            }
        }

        public void Button_OnMouseUp(object sender, MouseButtonEventArgs args) {
            if (clicking) {
                clicking = false;
                showPressed = false;
                Point point = args.GetPosition(Parent);
                if (RenderSlotRect.Contains(point)) {
                    RaiseEvent(ClickEvent, new RoutedEventArgs(this, ClickEvent));
                }
                ConsoleApplication.Instance.EndCaptureInput(this);
                this.Invalidate();
                args.Handled = true;
            }
        }
    }
}
