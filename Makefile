exe:
	make -C Pixie dll

all:
	make -C Pixie all

dll:
	make -C Pixie dll

flo:
	make -C Pixie flo

nuget:
	nuget restore Pixie.sln

clean: clean-ecsc
	make -C Pixie clean

include flame-make-scripts/use-ecsc.mk
