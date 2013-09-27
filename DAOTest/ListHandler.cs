using System;
using System.Collections.Generic;
using System.Web;
using DataUtils;

namespace DAOTest {

    public abstract class ListHandlerBase {
        public abstract object getContents();
        public abstract object getItem(int i);
        public abstract GenericDAO<X> getDAO<X>() where X : DataObject, new();
    }

    public class ListHandler<T> : ListHandlerBase where T : DataObject, new() {
        public override object getContents() {
            SqlDAO<T> dao = new SqlDAO<T>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", typeof(T));
            List<T> ret = dao.get("",0);
//            Loader.load<T>(ret, dao.emulate<DataObject>(typeof(T)));
            return ret;
        }

        public override object getItem(int i) {
            SqlDAO<T> dao = new SqlDAO<T>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", typeof(T));
            T ret = dao.get("",0)[i];
            if (ret != null)
                Loader.load(ret, dao.emulate<DataObject>(typeof(T)));
            return ret;
        }

        public override GenericDAO<X> getDAO<X>() {
            return new SqlDAO<X>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", typeof(T));
        }
    }

    public class TypeListHandler : ListHandlerBase {
        Type type_;
        public TypeListHandler(Type t) {
            type_ = t;
        }

        public override object getContents() {
            SqlDAO<DataObject> dao = new SqlDAO<DataObject>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", type_);
            return dao.get("",0);
        }

        public override object getItem(int i) {
            SqlDAO<DataObject> dao = new SqlDAO<DataObject>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", type_);
            return dao.get("",0)[i];
        }

        public override GenericDAO<X> getDAO<X>() {
            return new SqlDAO<X>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", type_);
        }
    }
}