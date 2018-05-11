using System;

namespace This.Model
{
    public abstract class Base<T> : IGeneric<T> where T : class
    {
        public void Delete()
        {
            throw new NotImplementedException();
        }

        public T Find(string id)
        {
            throw new NotImplementedException();
        }

        public void Id()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            this.GetType().GetProperty("Id").SetValue(this, "1");
        }

        public static T Find2(string id)
        {
            var Instance = Activator.CreateInstance<T>();
            var properties = Instance.GetType().GetProperties();
            Type type = Instance.GetType();

            throw new Exception("!");
            //return T;
        }
    }
}
