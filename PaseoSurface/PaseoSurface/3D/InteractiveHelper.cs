/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PanoEngine
{
    /// <summary>
    /// Clase estatica que nos permite saber cuando un puntero esta tocando en el espacio 3D ha un sprite.
    /// </summary>
    static class InteractiveHelper
    {
        /// <summary>
        /// Calcular el Ray que pasa por las coordenadas de pantalla pasadas por parametro.
        /// </summary>
        /// <param name="mouseLocation">Coordenadas en pantalla del puntero.</param>
        /// <param name="camera">Camara donde esta almacenada la matriz de proyeccion.</param>
        /// <param name="viewport">Viewport que se usa actualmente en la aplicacion.</param>
        /// <returns></returns>
        private static Ray CalculateRay(Vector2 mouseLocation, Camera camera, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f), camera.Projection, camera.View, Matrix.Identity);
            //System.Diagnostics.Debug.WriteLine("NearP " + nearPoint.ToString());
            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X, mouseLocation.Y, 1.0f), camera.Projection, camera.View, Matrix.Identity);
            //System.Diagnostics.Debug.WriteLine("DarP: " + farPoint.ToString());
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// Calcula la distancia entre el plano de corte y el objeto.
        /// </summary>
        /// <param name="sphere">Sphere que rodea el objeto de interaccion.</param>
        /// <param name="mouseLocation">Coordenadas en pantalla de puntero.</param>
        /// <param name="camera">Camara donde esta almacenada la matriz de proyeccion.</param>
        /// <param name="viewport">Viewport que se usa actualmente en la aplicacion.</param>
        /// <returns>Distancia entre el plano de corte y el objeto; null si no intersectan.</returns>
        private static float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation, Camera camera, Viewport viewport)
        {
            //Calcula el rayo y lo intersecta con la sphere
            Ray mouseRay = CalculateRay(mouseLocation, camera, viewport);
            return mouseRay.Intersects(sphere);
        }

        /// <summary>
        /// Averiguar si las coordenadas en pantalla estan encima del objeto.
        /// </summary>
        /// <param name="mouseLocation">Coordenadas en pantalla del puntero.</param>
        /// <param name="sprite">Sprite3D con el que se quiere interaccionar.</param>
        /// <param name="camera">Camara donde esta almacenada la matriz de proyeccion.</param>
        /// <param name="viewport">Viewport que se usa actualmente en la aplicacion.</param>
        /// <returns>True si intersectan; false en caso contrario.</returns>
        public static bool Intersects(Vector2 mouseLocation, Sprite3D sprite, Camera camera, Viewport viewport)
        {

            BoundingSphere sphere = sprite.GetBoundingSphere();
            //sphere = sphere.Transform(sprite.GetWorld());
            
            float? distance = IntersectDistance(sphere, mouseLocation, camera, viewport);
            //System.Diagnostics.Debug.WriteLine(sphere.Center + " " + sphere.Radius + " D: " + distance);
            if (distance != null)
            {
                return true;
            }

            return false;

        }
    }
}
*/