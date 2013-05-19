/** Class implementing a universally unique identifier per IETF RFC 4122, essentially a 128-bit
  * unsigned integer.
  *
  * @param hi the high-order bits of this [[Uuid]] value.
  * @param lo the low-order bits of this [[Uuid]] value.
  */
class Uuid(val hi:Long, val lo:Long) extends Ordered[Uuid] {
  /** The 32-bit `time_low` field of the UUID, as defined by RFC 4122. */
  def timeLow:Int = (hi >>> 32).toInt
  /** The 16-bit `time_mid` field of the UUID, as defined by RFC 4122. */
  def timeMid:Short = ((hi & 0x00000000FFFF0000l) >>> 16).toShort
  /** The 16-bit `time_hi_and_version` field of the UUID, as defined by RFC 4122. */
  def timeHiAndVersion:Short = (hi & 0x000000000000FFFFl).toShort
  /** The 8-bit `time_seq_hi_res` field of the UUID, as defined by RFC 4122. */
  def clkSeqHiRes:Byte = ((lo & 0xFF00000000000000l) >>> 56).toByte
  /** The 8-bit `clk_seq_low` field of the UUID, as defined by RFC 4122. */
  def clkSeqLow:Byte = ((lo & 0x00FF000000000000l) >>> 48).toByte
  /** The 48-bit `node` field of the UUID, as defined by RFC 4122. */
  def node:Long = lo & 0x0000FFFFFFFFFFFFl
  /** The 3-bit variant, as defined by RFC 4122. */
  def variant:Int = (lo >>> 61).toInt
  /** The 4-bit version, as defined by RFC 4122. */
  def version:Int = ((hi & 0x000000000000F000l) >>> 12).toInt
  /** The 60-bit timestamp encoded in the universally unique identifier by the `timeLow`,
    * `timeMid`, and `timeHiAndVersion` fields; the number of 100-nanosecond intervals elapsed
    * since 1582-10-15T00:00:00Z.
    * May not be meaningful for [[Uuid]] values which are not generated according to the version 1 algorithm
    * described in RFC 4122. */
  def timestamp:Long = 0x0FFFFFFFFFFFFFFFl & hiOrdered

  /** Retrieve the value of the [[Uuid]] as an `Array` of 16 `Byte` values. */
  def toByteArray =
    Array(hi, lo).flatMap({ x => Range.inclusive(56, 0, -8).map({ s => (x >>> s).toByte }) }).toArray

  /** Indicates whether this is the 'nil', 0-valued [[Uuid]]  defined by RFC 4122. */
  def isNil = 0 == hi && 0 == lo

  def compare(that:Uuid):Int = {
    def compareUnsigned(x:Long, y:Long):Int =
      if(x == y) 0 else (if((x < y) ^ (x < 0) ^ (y < 0)) -1 else 1)

    val x = compareUnsigned(this.hiOrdered, that.hiOrdered)
    if(0 != x) x else compareUnsigned(this.lo, that.lo)
  }

  private def hiOrdered = (hi << 48) | (0x0000FFFF00000000l & (hi << 16)) | (hi >>> 32)

  override def equals(obj:Any):Boolean =
    if(obj.isInstanceOf[Uuid]) {
      val that = obj.asInstanceOf[Uuid]
      hi == that.hi && lo == that.lo
    }
    else false

  override def hashCode():Int = timeLow

  /** Returns the string representation of the [[Uuid]] value as defined by RFC 4122. */
  override def toString:String = {
    @inline def h(i:Long):Char = Uuid.HexChar((i & 0xFl).toInt)

    new String(Array(
      h(hi >>> 60), h(hi >>> 56), h(hi >>> 52), h(hi >>> 48),
      h(hi >>> 44), h(hi >>> 40), h(hi >>> 36), h(hi >>> 32),
      '-',
      h(hi >>> 28), h(hi >>> 24), h(hi >>> 20), h(hi >>> 16),
      '-',
      h(hi >>> 12), h(hi >>>  8), h(hi >>>  4), h(hi),
      '-',
      h(lo >>> 60), h(lo >>> 56), h(lo >>> 52), h(lo >>> 48),
      '-',
      h(lo >>> 44), h(lo >>> 40), h(lo >>> 36), h(lo >>> 32),
      h(lo >>> 28), h(lo >>> 24), h(lo >>> 20), h(lo >>> 16),
      h(lo >>> 12), h(lo >>>  8), h(lo >>>  4), h(lo)
    ))
  }
}

object Uuid {
  private val HexChar:Array[Char] = "0123456789abcdef".toCharArray
  private val HexValue:Array[Long] =
    (new Array[Long]('f'+1) /: "0123456789AaBbCcDdEeFf") { (a, c) => a(c) = (0x0Fl & c) + ((0x40l & c) >>> 6) * 9l; a }

  /** The nil UUID, as specified by RFC 4122. */
  val nil = new Uuid(0l,0l)

  def apply(hi:Long, lo:Long) = new Uuid(hi, lo)

  /** Construct a new [[Uuid]] using the provided bytes as the value of the [[Uuid]].
    *
    * @param bs bytes used to initialize the value of the [[Uuid]]. Must have length 16.
    **/
  def apply(bs:Array[Byte]):Uuid = {
    assert(16 == bs.length, "Provided array must contain 16 bytes.")
    val hi = (0l /: bs.slice(0,  8)) { (x, b) => (x << 8) | (b & 0xFFl) }
    val lo = (0l /: bs.slice(8, 16)) { (x, b) => (x << 8) | (b & 0xFFl) }
    new Uuid(hi, lo)
  }

  private val UuidRegex = Array(8,4,4,4,12).view.map(n => s"([0-9a-fA-F]{$n})").mkString("^\\s*\\{?", "-?", "\\}?\\s*$").r

  /** Attempts to parse a [[Uuid]] from the provided string representation, optionally enclosed in curly braces.
    *
    * @param s The string to be parsed as a string representation of a [[Uuid]].
    * @return The parse Uuid, or `None` if `s` is not a valid string representation of a [[Uuid]].
    */
  def apply(s:String):Option[Uuid] = s match {
    // TODO: Should we accept any formulation making up the required 32 character hexadecimal string?
    case UuidRegex(tLow, tMid, tHi, clockSeq, node) => {
      val hi = (0l /: Seq.concat(tLow, tMid, tHi)) { (m, c) => (m << 4) | HexValue(c) }
      val lo = (0l /: Seq.concat(clockSeq, node)) { (m, c) => (m << 4) | HexValue(c) }
      Some(new Uuid(hi, lo))
    }
    case _ => Option.empty[Uuid]
  }

  private lazy val _sequentialGenerator = new SequentialUuidGenerator

  /** Generate a time-based [[Uuid]] value using the version 1 algorithm described in RFC 4122.
    * Values are generated using a shared [[SequentialUuidGenerator]] instance. */
  def sequential() = _sequentialGenerator.generate()
}

/** Facility for the generation of time-based [[Uuid]] values using the version 1 algorithm described in RFC 4122.
  * Generates sequential time-based [[Uuid]] values in a threadsafe manner.
  * Due to platform-specific restrictions, the `timestamp` value encoded in the resultant [[Uuid]] values is
  * limited to millisecond accuracy.
  * The remaining 5 digits of precision are provided by a meaningless incrementing counter. */
class SequentialUuidGenerator {
  private var _storedTimestamp = 0l
  private var _storedCount = 0l
  private var _storedLo = generateLo()

  /** Randomly generates the 64 low-order bits for a version 1 [[Uuid]], setting the reserved and multicast
    * bits as expected. */
  private def generateLo():Long =
    (0x3FFFFEFFFFFFFFFFl & java.util.UUID.randomUUID().getMostSignificantBits) |
    (0x8000010000000000l)

  /** Generates a new sequential [[Uuid]] value.
    * This method is threadsafe.
    * @return a new sequential [[Uuid]].
    */
  def generate():Uuid = {
    val (timestamp, lo) = synchronized {
      // Count of 100-nanosecond intervals since 1582-10-15T00:00:00Z
      val unixTime = new java.util.GregorianCalendar(SequentialUuidGenerator.UtcTimeZone).getTimeInMillis
      val t = unixTime * 10000l + 1221929280000000000l

      if(t == _storedTimestamp) {
        if(9999 == _storedCount) {
          // Handle possible overflow of our time counter (as might occur with a very fast computer)
          _storedLo = generateLo()
          _storedCount = 0
        } else {
          _storedCount += 1
        }
      } else {
        // If the current time has somehow decreased (as may be the case if the clock is changed), generate new random
        // clock sequence and node values to prevent possible collisions
        if(t < _storedTimestamp)
          _storedLo = generateLo()

        _storedTimestamp = t
        _storedCount = 0
      }

      (t + _storedCount, _storedLo)
    }

    new Uuid(
      (timestamp << 32) |
      (0x00000000FFFF0000l & (timestamp >>> 32)) |
      (0x0000000000000FFFl & (timestamp >>> 48)) |
      (0x0000000000001000l),
      lo
    )
  }
}

object SequentialUuidGenerator {
  private val UtcTimeZone = java.util.TimeZone.getTimeZone("UTC")
}

