object Levenshtein {
  def levenshtein[T](a:String, b:String):Int = {
    // This is a very non-idiomatic implementation, for performance reasons
    val al = a.length; val bl = b.length;
    if(0 == al) return bl;
    if(0 == bl) return al;

    val ds = bl + 1;
    var d0 = Array.range(0, ds);
    var d1 = new Array[Int](ds);

		var ai = 1; var aii = 0;
    while(ai <= al) {
      val ae = a.charAt(aii); // Cache the element of `a` under consideration

      d1(0) = ai;
			var bii = 0; var bi = 1;
      while(bi <= bl) {
        d1(bi) =
          if(ae == b.charAt(bii)) d0(bii)
          else {
            // This is a highly optimized `min` implementation
            val x = d0(bii); val y = d0(bi); val z = d1(bii);
            (if(x < y) (if(x < z) x else z) else (if(y < z) y else z)) + 1;
          }

        bii = bi; bi += 1;
      }

      // Swap `d1` and `d0` so that `d1` becomes the 'previous' row
      val dd = d0; d0 = d1; d1 = dd;
      aii = ai; ai += 1;
    }

    // The result is the last element of `d0`, which *was* `d1` before it was swapped
    d0(bl)
  }
}
