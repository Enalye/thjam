﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors {
	public static Color32 orchid = new Color32(218, 112, 214, 255);
	public static Color32 royalblue = new Color32(65, 105, 225, 255);
	public static Color32 chartreusegreen = new Color32(102, 205, 0, 255);
	public static Color32 firebrick = new Color32(178, 34, 34, 255);

	public static Color32 ChangeAlpha(Color32 c, byte a) {
		return new Color32(c.r, c.g, c.b, a);
	}
}
