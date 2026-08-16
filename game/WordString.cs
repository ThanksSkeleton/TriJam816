using System;

public class WordString
{
	private string inner;
	private int index;

	public WordString(string input)
	{
		this.inner = input.ToLowerInvariant().Trim();
	}

	public bool ApplyHit(char inputChar)
	{
		if (IsFinished())
		{
			throw new Exception("Hitting a finished word");
		} 
		else if (!char.IsLetter(inputChar))
		{
			throw new Exception("Not a letter "+ inputChar);
		}

		var lowerChar = char.ToLower(inputChar);
		if (inner[index] == lowerChar)
		{
			var inner_as_chars = inner.ToCharArray();
			inner_as_chars[index] = ' ';
			this.inner = new string(inner_as_chars);
			index++;
			return true;
		}

		return false;
	}

	public string Inner()
	{
		return this.inner;
	}

	public char FirstLetter()
	{
		if (!IsFinished())
		{
			return this.inner[index];
		} else
		{
			throw new Exception("Word is done, no first letter.");
		}
	}


	public bool IsFinished()
	{
		return this.inner.IsWhiteSpace();
	}
}

