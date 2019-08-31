namespace Xigadee
{
    public interface IMessageHeaderFragment
    {
        /// <summary>
        /// This property contains the field name.
        /// </summary>
        string Field { get; set; }
        /// <summary>
        /// This property contains the field data.
        /// </summary>
        string FieldData { get; set; }
    }
}
