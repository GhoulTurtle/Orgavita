public interface ISaveable{
    public ObjectData SaveData();
    public void LoadData(ObjectData objectData);
    public string GetObjectID();
}
