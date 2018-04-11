files = \
	*.cs

libraries = \
	../../Pixie/bin/clr/Pixie.dll \
	../../Pixie.Terminal/bin/clr/Pixie.Terminal.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=PrintHelp -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo exe clean
all: flo exe

flo: bin/flo/PrintHelp.flo
exe: bin/clr/PrintHelp.exe

clean:
	rm -rf bin/
	rm -rf obj/

include ../../flame-make-scripts/use-ecsc.mk

bin/flo/PrintHelp.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/PrintHelp.exe: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -libs $(libraries) -o $@ $(common_options)
