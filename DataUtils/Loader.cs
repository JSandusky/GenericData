using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DataUtils {
    // Deals with loading in aggregates
    public class Loader {
        public static void load(DataObject obj, GenericDAO<DataObject> dao) {
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in props) {
                if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                    DataObject cur = pi.GetValue(obj,null) as DataObject;
                    if (cur != null) {
                        DataObject newDao = dao.emulate(pi.PropertyType).getSingle("WHERE _dbId = " + cur._dbId);
                        pi.SetValue(obj, newDao, null);
                    }
                } else if (pi.PropertyType == typeof(byte[])) {
                    //DO NOTHING
                } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                    IList li = pi.GetValue(obj, null) as IList;
                    Type contType = pi.PropertyType.GetGenericArguments()[0];
                    if (li == null)
                        continue;
                    for (int i = 0; i < li.Count; ++i) {
                        DataObject inst = li[i] as DataObject;
                        inst = dao.emulate(contType).getSingle("WHERE _dbId = " + inst._dbId);
                        if (inst != null)
                            li[i] = inst;
                        else {
                            li.RemoveAt(i);
                            --i;
                        }
                    }
                }
            }
        }

        public static void load<T>(List<T> obj, GenericDAO<DataObject> dao) where T : DataObject {
            foreach (T thing in obj)
                load(thing, dao);
        }

        public static void save(DataObject obj, GenericDAO<DataObject> dao) {
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in props) {
                if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                    DataObject cur = pi.GetValue(obj, null) as DataObject;
                    if (cur != null)
                        dao.emulate(pi.PropertyType).saveOrUpdate(cur);
                } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                    IList li = pi.GetValue(obj, null) as IList;
                    Type contType = pi.PropertyType.GetGenericArguments()[0];
                    for (int i = 0; i < li.Count; ++i) {
                        DataObject inst = li[i] as DataObject;
                        if (inst != null) {
                            save(inst, dao);
                            dao.emulate(contType).saveOrUpdate(inst);
                        }
                    }
                }
            }
        }
    }
}
