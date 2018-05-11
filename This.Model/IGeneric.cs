namespace This.Model
{
    public interface IGeneric<T> where T : class
    {
        void Id();
        void Save();
        void Delete();
        T Find(string id);
    }
}
