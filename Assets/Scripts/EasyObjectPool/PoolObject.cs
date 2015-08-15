/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;

namespace MarchingBytes {
	public class PoolObject : MonoBehaviour {
		public string poolName;
		//defines whether the object is waiting in pool or is in use
		public bool isPooled;
	}
}
