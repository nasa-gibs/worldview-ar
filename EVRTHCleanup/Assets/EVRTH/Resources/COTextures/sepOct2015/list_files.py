import os

outfile = open('2015_texture_files.txt', 'w')
for filename in os.listdir("."):
    if filename.endswith(".png"):
        outfile.write(filename + '\n')