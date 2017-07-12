namespace StoredProcedure
{
    public class StoredProcedureParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public StoredProcedureParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
