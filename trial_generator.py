#python
from random import randint
from random import random
import math as math

#Create a random trial file


# print("a={0},b={1}".format(a, b))


def setup_file(lat, lon, zoom, mercX, mercY, features_string, outfile, pname, radius, bc_string, num_points):
	outString = ""
	outString += "@Lat\t{0}\n".format(lat)
	outString += "@Lon\t{0}\n".format(lon)
	outString += "@Zoom\t{0}\n".format(zoom)
	outString += "@mercX\t{0}\n".format(mercX)
	outString += "@mercY\t{0}\n".format(mercY)
	outString += "@functions\t{0}\n".format(features_string)
	outString += "@outfile\t{0}\n".format(outfile)
	outString += "@pname\t{0}\n".format(pname);
	
	for i in range(num_points):
		r = randint(0,radius)
		theta = random() * 2*math.pi
		x = r * math.cos(theta) + radius
		z = r * math.sin(theta) + radius
		o = random() * 2 + 1
		outString += "@coin\tid\t{0}\tx\t{1}\tz\t{2}\toffset\t{3}\t{4}\n".format(i,x,z,o,bc_string)
	
	return outString
	
	
s = setup_file(51.17, -115.31, 11, 367, 383, "frtm", "sample_gen.csv","dev", 512, "c", 10)
print(s)