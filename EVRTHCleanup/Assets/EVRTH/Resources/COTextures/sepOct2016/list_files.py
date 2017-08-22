import os

outfile = open('2016_texture_files.txt', 'w')
for filename in os.listdir("."):
    if filename.endswith(".png"):
        outfile.write(filename + '\n')