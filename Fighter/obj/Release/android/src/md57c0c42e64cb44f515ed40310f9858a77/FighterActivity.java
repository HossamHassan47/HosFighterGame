package md57c0c42e64cb44f515ed40310f9858a77;


public class FighterActivity
	extends md54575049b9fa2b2042ef5a8fa79e8ef54.AndroidGameActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Fighter.FighterActivity, Fighter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FighterActivity.class, __md_methods);
	}


	public FighterActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == FighterActivity.class)
			mono.android.TypeManager.Activate ("Fighter.FighterActivity, Fighter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
