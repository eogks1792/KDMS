namespace KDMSViewer.Interface
{
    public interface IAbstractFactory<T>
    {
        T Create();
        void Close();
    }
}