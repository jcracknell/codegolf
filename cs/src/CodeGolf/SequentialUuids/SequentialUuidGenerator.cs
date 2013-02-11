using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.SequentialUuids {
	/// <summary>
	/// Facility for the generation and manipulation of sequential version 1 UUID values and equivalent <see cref="Guid"/> representations per RFC 4122.
	/// </summary>
	public class SequentialUuidGenerator {
		private const uint MaxSequence = 0x3FFF;
		private static readonly long October_15_1582 = new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc).Ticks;

		private ulong _storedTimestamp;
		private uint _storedSequence;
		private readonly Random _random;
		private readonly byte[] _storedNode = new byte[6];

		/// <summary>
		/// Create a new <see cref="SequentialUuidGenerator"/>.
		/// </summary>
		public SequentialUuidGenerator() {
			// This is an expensive but effective way of seeding a random number generator;
			// should ensure that constructing two instances consecutively always results in a unique seed
			var seedBytes = Guid.NewGuid().ToByteArray();
			_random = new Random((seedBytes[1] << 24) | (seedBytes[3] << 16) | (seedBytes[13] << 8) | (seedBytes[15]));

			// Generate the initial node id
			GenerateNodeId();
		}

		private void GenerateNodeId() {
			_random.NextBytes(_storedNode);
			// Set multicast bit per RFC 4122
			_storedNode[0] |= 1;
		}

		/// <summary>
		/// Generate a sequential UUID as a byte array.
		/// </summary>
		public byte[] GenerateBytes() {
			byte[] uuid = new byte[16];
			uint sequence = 0;
			ulong timestamp;

			lock(_random) {
				timestamp = (ulong)(DateTime.Now.ToUniversalTime().Ticks - October_15_1582);

				if(timestamp == _storedTimestamp) {
					sequence = ++_storedSequence;
				} else {
					// If the time has changed BACKWARDS (as might occur when the system time is changed),
					// generate a new node id to prevent possible collisions
					if(timestamp < _storedTimestamp)
						GenerateNodeId();

					_storedTimestamp = timestamp;
					_storedSequence = 0;
				}

				uuid[10] = _storedNode[0];
				uuid[11] = _storedNode[1];
				uuid[12] = _storedNode[2];
				uuid[13] = _storedNode[3];
				uuid[14] = _storedNode[4];
				uuid[15] = _storedNode[5];
			}

			// time_low
			uuid[0] = (byte)((timestamp >> 24));
			uuid[1] = (byte)((timestamp >> 16));
			uuid[2] = (byte)((timestamp >> 8));
			uuid[3] = (byte)((timestamp));
			// time_mid
			uuid[4] = (byte)((timestamp >> 40));
			uuid[5] = (byte)((timestamp >> 32));
			// time_hi_and_version
			uuid[6] = (byte)((timestamp >> 56 & 0x0Ful) | 0x10ul);
			uuid[7] = (byte)((timestamp >> 48));
			// clock_seq_hi_and_reserved
			uuid[8] = (byte)((sequence >> 8 & 0x3Ful) | 0xE0ul);
			// clock_seq_low
			uuid[9] = (byte)((sequence));

			return uuid;
		}

		/// <summary>
		/// Generate a sequential UUID as a <see cref="Guid"/>.
		/// </summary>
		public Guid GenerateGuid() {
			return new Guid(GuidByteOrder(GenerateBytes()));
		}

		/// <summary>
		/// Convert the provided byte array to or from the order used by the <see cref="Guid"/> struct.
		/// </summary>
		public static byte[] GuidByteOrder(byte[] bytes) {
			if(null == bytes) throw Xception.Because.ArgumentNull(() => bytes);

			return new byte[16] {
				bytes[3], bytes[2], bytes[1], bytes[0],
				bytes[5], bytes[4],
				bytes[7], bytes[6],
				bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]
			};
		}

		/// <summary>
		/// Extract the UTC timestamp contained in the provided byte array representation of a sequential UUID.
		/// </summary>
		public static DateTime ExtractTimestamp(byte[] bytes) {
			if(null == bytes) throw Xception.Because.ArgumentNull(() => bytes);
			if(!IsSequentialUuid(bytes)) throw Xception.Because.Argument(() => bytes, "must be a sequential UUID");

			long timestamp = 
				  ((long)(bytes[6] & 0x0F) << 56)
				| ((long)bytes[7] << 48)
				| ((long)bytes[4] << 40)
				| ((long)bytes[5] << 32)
				| ((long)bytes[0] << 24)
				| ((long)bytes[1] << 16)
				| ((long)bytes[2] << 8)
				| ((long)bytes[3]);

			return new DateTime(timestamp + October_15_1582, DateTimeKind.Utc);
		}

		/// <summary>
		/// Extract the UTC timestamp contained in the provided <see cref="Guid"/> representation of a sequential UUID.
		/// </summary>
		public static DateTime ExtractTimestamp(Guid guid) {
			return ExtractTimestamp(GuidByteOrder(guid.ToByteArray()));
		}

		/// <summary>
		/// Returns true if the provided byte array is a valid sequential UUID.
		/// </summary>
		public static bool IsSequentialUuid(byte[] bytes) {
			return null != bytes
				&& 16 == bytes.Length
				&& 0x10 == (bytes[6] & 0xF0);
		}

		/// <summary>
		/// Returns true if the provided <see cref="Guid"/> is a valid sequential UUID.
		/// </summary>
		public static bool IsSequentialUuid(Guid guid) {
			return IsSequentialUuid(GuidByteOrder(guid.ToByteArray()));
		}
	}
}
