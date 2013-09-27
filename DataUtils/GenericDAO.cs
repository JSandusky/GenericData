using System;
using System.Collections.Generic;
using System.Text;

namespace DataUtils {
    public abstract class GenericDAO<T> where T : DataObject {
        protected TypeHandler TypeHandler { get; set; }
        
        public GenericDAO(TypeHandler aTyper) {
            TypeHandler = aTyper;
        }
        public abstract T getSingle(string query);
        public abstract List<T> get(string query, int ct);
        public abstract void saveOrUpdate(T obj);
        public abstract void delete(T obj);
        public abstract GenericDAO<T> emulate(Type t);
        public abstract GenericDAO<X> emulate<X>(Type t) where X : DataObject, new();
    }
}
