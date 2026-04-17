using Android.App;
using Android.Views;
using AndroidWidget = Android.Widget;
using AndroidView = Android.Views.View;
using AndroidRect = Android.Graphics.Rect;

namespace App_CrediVnzl.Platforms.Android
{
    public static class KeyboardHelper
    {
        private static AndroidView? rootView;
        private static int originalHeight;
        private static bool isKeyboardListenerAttached;

        public static void Initialize(Activity activity)
        {
            if (activity?.Window?.DecorView?.RootView == null || isKeyboardListenerAttached)
                return;

            rootView = activity.Window.DecorView.RootView;
            originalHeight = 0;
            isKeyboardListenerAttached = true;

            // Configurar listener para cambios de layout que detecta el teclado
            rootView.ViewTreeObserver?.AddOnGlobalLayoutListener(new KeyboardLayoutListener());

            System.Diagnostics.Debug.WriteLine("*** KeyboardHelper: Listener configurado ***");
        }

        private class KeyboardLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            public void OnGlobalLayout()
            {
                if (rootView is null) return;

                var rect = new AndroidRect();
                rootView.GetWindowVisibleDisplayFrame(rect);

                int screenHeight = rootView.RootView?.Height ?? 0;
                int keypadHeight = screenHeight - rect.Bottom;

                // Si el teclado ocupa m�s del 15% de la pantalla, se considera visible
                bool isKeyboardVisible = keypadHeight > screenHeight * 0.15;

                if (isKeyboardVisible)
                {
                    System.Diagnostics.Debug.WriteLine($"*** TECLADO VISíBLE: altura {keypadHeight}px ***");
                    
                    // Obtener el view enfocado actual
                    var currentFocus = (rootView.Context as Activity)?.CurrentFocus;
                    if (currentFocus != null)
                    {
                        ScrollToView(currentFocus, keypadHeight);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** TECLADO OCULTO ***");
                }
            }

            private void ScrollToView(AndroidView focusedView, int keyboardHeight)
            {
                // Buscar el ScrollView padre
                var scrollView = FindScrollViewParent(focusedView);
                if (scrollView is null) return;

                // Calcular la posici�n del view enfocado
                var location = new int[2];
                focusedView.GetLocationOnScreen(location);
                int viewTop = location[1];
                int viewBottom = viewTop + focusedView.Height;

                // Calcular el espacio visible (pantalla - teclado)
                int visibleHeight = (rootView?.Height ?? 0) - keyboardHeight;

                // Si el campo est� oculto por el teclado, hacer scroll
                if (viewBottom > visibleHeight)
                {
                    int scrollAmount = viewBottom - visibleHeight + 100; // +100px de margen
                    scrollView.Post(() => 
                    {
                        scrollView.SmoothScrollBy(0, scrollAmount);
                        System.Diagnostics.Debug.WriteLine($"*** ScrollView ajustado: +{scrollAmount}px ***");
                    });
                }
            }

            private AndroidWidget.ScrollView? FindScrollViewParent(AndroidView view)
            {
                var parent = view.Parent as AndroidView;
                while (parent != null)
                {
                    if (parent is AndroidWidget.ScrollView scrollView)
                        return scrollView;
                    
                    parent = parent.Parent as AndroidView;
                }
                return null;
            }
        }
    }
}

