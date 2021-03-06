#region license

//  Copyright (C) 2018 ClassicUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies.  
//      
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using ClassicUO.Game.GameObjects;
using ClassicUO.Input;
using ClassicUO.Renderer;

using Microsoft.Xna.Framework;

namespace ClassicUO.Game.Views
{
    public class DeferredView : View
    {
        private readonly View _baseView;
        private readonly Vector3 _position;

        public DeferredView(DeferredEntity deferred, View baseView, Vector3 position) : base(deferred)
        {
            _baseView = baseView;
            _position = position;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 position, MouseOverList objectList)
        {
            //if (_baseView.GameObject is Mobile mobile)
            //{
            //    if (mobile.IsDead || mobile.IsDisposed || mobile.Graphic == 0)
            //    {
            //        GameObject.Dispose();
            //        return false;
            //    }
            //}

            return _baseView.DrawInternal(spriteBatch, _position, objectList);
        }
    }
}