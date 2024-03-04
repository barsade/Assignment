Code:
#include <linux/module.h>
#include <linux/binfmts.h>
#include <linux/fs.h>
#include <linux/slab.h>
#include <linux/elf.h>

#define XOR_KEY 'A'

int do_load_xor_encrypted_elf(struct linux_binprm* bprm);
int validate_encrypted_file_magic(struct linux_binprm* bprm);
ssize_t change_to_elf_magic_numbers(struct file* file, loff_t* offset);
int decrypt_elf(struct linux_binprm* bprm);

int load_xor_encrypted_elf(struct linux_binprm* bprm) {
    printk(KERN_INFO "Test!\n");
    int retval = 0;
    retval = do_load_xor_encrypted_elf(bprm);
    return retval;
}

int do_load_xor_encrypted_elf(struct linux_binprm* bprm) {
    if (!validate_encrypted_file_magic(bprm)) {
        printk(KERN_INFO "File format is either not encrypted or 4 magic numbers are incompatible\n");
        return -EINVAL; // Indicate invalid argument
    }

    if (!decrypt_elf(bprm)) {
        printk(KERN_ERR "Failed to decrypt the ELF file. \n");
        return -EINVAL; // Indicate invalid argument
    }

    ssize_t result = change_to_elf_magic_numbers(bprm->file, 0);
    if (result < 4) {
        printk(KERN_ERR "Failed to write ELF magic numbers: %zd\n", result);
        return -EIO;
    }

    return 0; // Indicate success
}

int validate_encrypted_file_magic(struct linux_binprm* bprm) {
    // Unique 4 magic numbers - stands for Xor Encrypted ELF
    unsigned char expectedMagic[] = { 0x7F, 'X', 'E', 'E' };
    unsigned char* buf = bprm->buf;
    int i;

    // Assuming the magic numbers are within the first bytes read into bprm->buf
    for (i = 0; i < 4; i++) {
        if ((buf[i] ^ XOR_KEY) != expectedMagic[i]) {
            printk(KERN_ERR "Magic number is not compatible with the current format %d\n", i);
            return -EINVAL;
        }
    }

    printk(KERN_INFO "The magic numbers match 'X', 'E', 'E', 'F' after decryption.\n");
    return 1;
}

ssize_t change_to_elf_magic_numbers(struct file* file, loff_t* offset) {
    char elf_magic[4] = { 0x7F, 'E', 'L', 'F' };
    ssize_t written;

    written = kernel_write(file, elf_magic, sizeof(elf_magic), offset);
    if (written != sizeof(elf_magic)) {
        printk(KERN_ERR "Failed to write all ELF magic bytes. Written: %zd\n", written);
        return -EIO;
    }

    return written; // Return the number of bytes written (should be 4)
}

int decrypt_elf(struct linux_binprm* bprm) {
    unsigned char xor_key = XOR_KEY;
    unsigned char* buf;
    loff_t pos = 0;
    size_t bytes_read;
    size_t bytes_written;

    buf = kmalloc(PAGE_SIZE, GFP_KERNEL);
    if (!buf) {
        printk(KERN_ERR "Failed to allocate memory\n");
        return -ENOMEM;
    }

    while ((bytes_read = kernel_read(bprm->file, buf, PAGE_SIZE, &pos)) > 0) {
        for (int i = 0; i < bytes_read; i++)
            buf[i] ^= xor_key;

        // Write decrypted data back to file
        bytes_written = kernel_write(bprm->file, buf, bytes_read, &pos);
        if (bytes_written < 0) {
            printk(KERN_ERR "Failed to write decrypted data\n");
            kfree(buf);
            return bytes_written;
        }

        if (bytes_written != bytes_read) {
            printk(KERN_ERR "Incomplete write during decryption\n");
            kfree(buf);
            return -EIO;
        }
    }

    // Check for read error
    if (bytes_read < 0) {
        printk(KERN_ERR "Failed to read data\n");
        kfree(buf);
        return bytes_read;
    }

    kfree(buf);
    return 0;
}

// Define the binary format structure
static struct linux_binfmt xor_encrypted_elf_format = {
    .module = THIS_MODULE,
    .load_binary = load_xor_encrypted_elf,
};

static int init_xor_encrypted_elf_loader(void) {
    register_binfmt(&xor_encrypted_elf_format);
    return 0;
}

static void exit_xor_encrypted_elf_loader(void) {
    unregister_binfmt(&xor_encrypted_elf_format);
}

module_init(init_xor_encrypted_elf_loader);
module_exit(exit_xor_encrypted_elf_loader);




Makefile:
obj-m += testing.o

all:
    make -C /lib/modules/$(shell uname -r)/build M=$(PWD) modules EXTRA_CFLAGS="-std=gnu99"
clean:
    make -C /lib/modules/$(shell uname -r)/build M=$(PWD) clean
