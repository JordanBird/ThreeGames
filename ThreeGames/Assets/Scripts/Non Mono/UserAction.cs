using UnityEngine;
using System.Collections;

public class UserAction
{
	public delegate void RunAction();

	public string name = "";
	public RunAction runAction;

	public UserAction(string name, RunAction method)
	{
		this.name = name;
		this.runAction = method;
	}
}
