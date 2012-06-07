/**************************************************************
 This file is part of Kinect Sensor Architecture Development Project.

   Kinect Sensor Architecture Development Project is free software:
   you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   Kinect Sensor Architecture Development Project is distributed in
   the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Kinect Sensor Architecture Development Project.  If
   not, see <http://www.gnu.org/licenses/>.
**************************************************************/
/**************************************************************
The work was done in joint collaboration with Cisco Systems Inc.
Copyright © 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

using System;
using System.Reflection;

namespace NuiDeviceFramework.reflection
{
    public static class ReflectionUtilities
    {
        public static object InvokeMethod(object o, String method, object[] parameters)
        {
            if (o == null || method == null)
            {
                return null;
            }
            MethodInfo[] methods = o.GetType().GetMethods();
            foreach (MethodInfo m in methods)
            {
                if (m.Name.Equals(method))
                {
                    object val = m.Invoke(o, parameters);
                    return val;
                }
            }
            return null;
        }

        public static object InvokeProperty(object o, String property)
        {
            if (o == null || property == null)
            {
                return null;
            }
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                if (p.Name.Equals(property))
                {
                    object val = p.GetValue(o, null);
                    return val;
                }
            }
            return null;
        }
    }
}
