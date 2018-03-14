[System.Serializable]
public class CustomReference<T>
{
	public bool UseConstant = true;
	public T ConstantValue;
	public CustomVariable<T> Variable;

	public T Value {
		get { return UseConstant ? ConstantValue : Variable.Value; }
	}
}
