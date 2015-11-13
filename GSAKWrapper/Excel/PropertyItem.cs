using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItem
    {
        private static List<PropertyItem> _propertyItems = null;
        private static object _lockObject = new object();

        public static List<PropertyItem> PropertyItems
        {
            get
            {
                List<PropertyItem> result = null;
                lock (_lockObject)
                {
                    if (_propertyItems == null)
                    {
                        _propertyItems = new List<PropertyItem>();
                        Assembly asm = Assembly.GetExecutingAssembly();
                        Type[] types = asm.GetTypes();
                        foreach (Type t in types)
                        {
                            if (t.BaseType == typeof(PropertyItem))
                            {
                                ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
                                var obj = (PropertyItem)constructor.Invoke(null);
                                _propertyItems.Add(obj);
                            }
                        }
                    }
                    result = _propertyItems;
                }
                return result;
            }
        }

        public string Name { get; private set; }
        public string Text { get { return ToString(); } }

        public PropertyItem(string name)
        {
            Name = name;
        }

        public virtual object GetValue(GeocacheData gc)
        {
            return null;
        }

        public override string ToString()
        {
            return Localization.TranslationManager.Instance.Translate(Name) as string;
        }
    }
}
